using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Razensoft.Faktory.Resp;
using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class FaktoryConnection : IDisposable
    {
        private readonly IConnectionConfiguration configuration;
        private readonly string id = Guid.NewGuid().ToString();
        private RespReader reader;
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
            reader = new RespReader(stream);
            respWriter = new RespWriter(stream);
            var message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Hi)
                throw new Exception($"Received unexpected handshake verb {message.Verb}");
            var handshake = message.Deserialize<HandshakeRequestDto>();
            if (handshake.Version > configuration.Identity.ProtocolVersion)
            {
                Console.WriteLine("Faktory protocol has been upgraded recently. Please, upgrade worker library.");
                Environment.Exit(0);
            }
            if (handshake.Nonce != null)
                configuration.Identity.PasswordHash =
                    GetPasswordHash(configuration.Password, handshake.Nonce, handshake.HashIterations);
            await SendAsync(new FaktoryMessage(MessageVerb.Hello, configuration.Identity.ToHandshake()));
            message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Ok)
                throw new Exception($"Received unexpected handshake confirmation verb {message.Verb}");
        }

        public static string GetPasswordHash(string password, string nonce, int iterations)
        {
            var data = Encoding.ASCII.GetBytes(password + nonce);
            var sha = SHA256.Create();
            for (var i = 0; i < iterations; i++)
                data = sha.ComputeHash(data);
            sha.Dispose();
            return string.Concat(data.Select(b => b.ToString("x2")));
        }

        public async Task<FaktoryMessage> ReceiveAsync()
        {
            var respMessage = await reader.ReadAsync();

            switch (respMessage)
            {
                case SimpleStringMessage simpleString:
                    Console.WriteLine($"{id} - receiving {simpleString.Payload}");
                    return new FaktoryMessage(simpleString);
                case BulkStringMessage bulkString:
                    Console.WriteLine($"{id} - receiving {bulkString.Payload}");
                    return new FaktoryMessage(bulkString);
            }
            return new FaktoryMessage(MessageVerb.Unknown, null);
        }

        public async Task SendAsync(FaktoryMessage message)
        {
            var line = $"{message.Verb.ToString().ToUpper()} {message.Payload}";
            Console.WriteLine($"{id} - sending {line}");
            var respMessage = new InlineCommandMessage(line);
            await respWriter.WriteAsync(respMessage);
        }
    }
}