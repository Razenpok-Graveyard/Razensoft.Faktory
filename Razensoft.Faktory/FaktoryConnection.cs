using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Razensoft.Faktory.Resp;

namespace Razensoft.Faktory
{
    public class FaktoryConnection: IDisposable
    {
        private readonly IConnectionConfiguration configuration;
        private IConnectionTransport transport;
        private bool isDisposed;
        private RespReader reader;
        private RespWriter respWriter;
        private StreamWriter streamWriter;

        public FaktoryConnection(IConnectionConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public FaktoryConnection(): this(new FaktoryConnectionConfiguration()) { }

        public async Task ConnectAsync()
        {
            transport = configuration.TransportFactory.CreateTransport();
            var stream = await transport.GetStream();
            var streamReader = CreateStreamReader(stream);
            reader = new RespReader(streamReader);
            streamWriter = CreateStreamWriter(stream);
            respWriter = new RespWriter(streamWriter);
            var message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Hi)
                throw new Exception("Whoopsie");
            Console.WriteLine($"HI {message.Payload}");
            // TODO: proper handling + version
            var handshake = message.Deserialize<FaktoryHandshake>();
            if (handshake.Nonce != null)
                configuration.Identity.PasswordHash = GetPasswordHash(configuration.Password, handshake.Nonce);
            await SendAsync(new FaktoryMessage(MessageVerb.Hello, configuration.Identity));
            message = await ReceiveAsync();
            if (message.Verb != MessageVerb.Ok)
                throw new Exception("Whoopsie");
            Console.WriteLine("OK");
            MaintainHeartBeatAsync();
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

        private async void MaintainHeartBeatAsync()
        {
            var message = new FaktoryMessage(MessageVerb.Beat, configuration.Identity);
            while (!isDisposed)
            {
                await SendAsync(message);
                await Task.Delay(configuration.HeartBeatPeriod);
                // TODO: response
            }
        }

        private static StreamReader CreateStreamReader(Stream stream)
        {
            return new StreamReader(stream, Encoding.ASCII, true, 1024, true);
        }

        private static StreamWriter CreateStreamWriter(Stream stream)
        {
            return new StreamWriter(stream, Encoding.ASCII, 1024, true)
            {
                AutoFlush = false,
                NewLine = "\r\n",
            };
        }

        public async Task<FaktoryMessage> ReceiveAsync()
        {
            var respMessage = await reader.ReadAsync();
            switch (respMessage)
            {
                case SimpleStringMessage simpleString:
                    return new FaktoryMessage(simpleString);
                case BulkStringMessage bulkString:
                    return new FaktoryMessage(bulkString);
            }
            return new FaktoryMessage(MessageVerb.Unknown, null);
        }

        public async Task SendAsync(FaktoryMessage message)
        {
            var line = $"{message.Verb.ToString().ToUpper()} {message.Payload}";
            var respMessage = new InlineCommandMessage(line);
            await respWriter.WriteAsync(respMessage);
            await streamWriter.FlushAsync();
        }

        public void Dispose()
        {
            isDisposed = true;
            transport.Dispose();
        }
    }
}