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
        private RespReader reader;
        private RespWriter respWriter;
        private IConnectionTransport transport;
        private readonly string id = Guid.NewGuid().ToString();

        public FaktoryConnection(IConnectionConfiguration configuration) => this.configuration = configuration;

        public void Dispose()
        {
            transport.Dispose();
        }

        public async Task ConnectAsync()
        {
            transport = configuration.TransportFactory.CreateTransport();
            var stream = await transport.GetStream();
            reader = new RespReader(stream);
            respWriter = new RespWriter(stream);
            var message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Hi)
                throw new Exception("Whoopsie");
            // TODO: proper version handling
            var handshake = message.Deserialize<HandshakeRequestDto>();
            if (handshake.Nonce != null)
                configuration.Identity.PasswordHash = GetPasswordHash(configuration.Password, handshake.Nonce);
            await SendAsync(new FaktoryMessage(MessageVerb.Hello, new HandshakeResponseDto(configuration.Identity)));
            message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Ok)
                throw new Exception("Whoopsie");
        }

        private static string GetPasswordHash(string password, string nonce)
        {
            using (var sha = SHA256.Create())
            {
                var encoding = Encoding.ASCII;
                var bytes = encoding.GetBytes(password + nonce);
                var hash = sha.ComputeHash(bytes);
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
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