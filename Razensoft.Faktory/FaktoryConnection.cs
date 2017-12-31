using System;
using System.Threading.Tasks;
using Razensoft.Faktory.Logging;
using Razensoft.Faktory.Resp;
using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class FaktoryConnection : IDisposable
    {
        private static Log Log { get; } = new Log(typeof(FaktoryConnection));

        private const int ProtocolVersion = 2;

        private readonly IConnectionConfiguration configuration;
        private readonly Guid id = Guid.NewGuid();
        private RespReader respReader;
        private RespWriter respWriter;
        private IConnectionTransport transport;

        public FaktoryConnection(IConnectionConfiguration configuration) => this.configuration = configuration;

        public void Dispose()
        {
            transport.Dispose();
        }

        public async Task ConnectAsync()
        {
            await SetupTransportAsync();
            var handshake = await ReceiveHandshakeAsync();
            var handshakeResponse = CreateHandshakeResponse(handshake);
            await SendHandshakeResponseAsync(handshakeResponse);
        }

        private async Task SetupTransportAsync()
        {
            transport = configuration.TransportFactory.CreateTransport();
            var stream = await transport.GetStreamAsync();
            respReader = new RespReader(stream);
            respWriter = new RespWriter(stream);
        }

        private async Task<HandshakeRequestDto> ReceiveHandshakeAsync()
        {
            var message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Hi)
                throw new Exception($"Received unexpected handshake verb {message.Verb}");
            var handshake = message.Deserialize<HandshakeRequestDto>();
            if (handshake.Version > ProtocolVersion)
            {
                Log.Error("Faktory protocol has been upgraded recently. Please, upgrade worker library.");
                Environment.Exit(0);
            }
            return handshake;
        }

        private HandshakeResponseDto CreateHandshakeResponse(HandshakeRequestDto handshake)
        {
            var handshakeResponse = configuration.Identity.CreateHandshake();
            handshakeResponse.Version = ProtocolVersion;
            if (handshake.Nonce != null)
                handshakeResponse.PasswordHash =
                    configuration.Password.GetHash(handshake.Nonce, handshake.HashIterations);
            return handshakeResponse;
        }

        private async Task SendHandshakeResponseAsync(HandshakeResponseDto handshakeResponse)
        {
            await SendAsync(new FaktoryMessage(MessageVerb.Hello, handshakeResponse));
            var message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Ok)
                throw new Exception($"Received unexpected handshake confirmation verb {message.Verb}");
        }

        public async Task<FaktoryMessage> ReceiveAsync()
        {
            var respMessage = await respReader.ReadAsync();

            switch (respMessage)
            {
                case SimpleStringMessage simpleString:
                    Trace($"Received {simpleString.Payload}");
                    return new FaktoryMessage(simpleString);
                case BulkStringMessage bulkString:
                    Trace($"Received {bulkString.Payload}");
                    return new FaktoryMessage(bulkString);
            }
            return new FaktoryMessage(MessageVerb.Unknown, null);
        }

        public async Task SendAsync(FaktoryMessage message)
        {
            var line = $"{message.Verb.ToString().ToUpper()} {message.Payload}";
            Trace($"Sending {line}");
            var respMessage = new InlineCommandMessage(line);
            await respWriter.WriteAsync(respMessage);
        }

        private void Trace(string message)
        {
            Log.Trace($"Connection {id}: {message}");
        }
    }
}