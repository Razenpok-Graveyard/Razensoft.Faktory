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
            transport = configuration.TransportFactory.CreateTransport();
            var stream = await transport.GetStreamAsync();
            respReader = new RespReader(stream);
            respWriter = new RespWriter(stream);
            var message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Hi)
                throw new Exception($"Received unexpected handshake verb {message.Verb}");
            var handshake = message.Deserialize<HandshakeRequestDto>();
            if (handshake.Version > configuration.Identity.ProtocolVersion)
            {
                Log.Error("Faktory protocol has been upgraded recently. Please, upgrade worker library.");
                Environment.Exit(0);
            }

            if (handshake.Nonce != null)
                configuration.Identity.PasswordHash =
                    configuration.Password.GetHash(handshake.Nonce, handshake.HashIterations);
            await SendAsync(new FaktoryMessage(MessageVerb.Hello, configuration.Identity.ToHandshake()));
            message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Ok)
                throw new Exception($"Received unexpected handshake confirmation verb {message.Verb}");
        }

        public async Task<FaktoryMessage> ReceiveAsync()
        {
            var respMessage = await respReader.ReadAsync();

            switch (respMessage)
            {
                case SimpleStringMessage simpleString:
                    Log.Trace($"{id} - receiving {simpleString.Payload}");
                    return new FaktoryMessage(simpleString);
                case BulkStringMessage bulkString:
                    Log.Trace($"{id} - receiving {bulkString.Payload}");
                    return new FaktoryMessage(bulkString);
            }
            return new FaktoryMessage(MessageVerb.Unknown, null);
        }

        public async Task SendAsync(FaktoryMessage message)
        {
            var line = $"{message.Verb.ToString().ToUpper()} {message.Payload}";
            Log.Trace($"{id} - sending {line}");
            var respMessage = new InlineCommandMessage(line);
            await respWriter.WriteAsync(respMessage);
        }
    }
}