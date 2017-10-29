using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Razensoft.Faktory.Resp;

namespace Razensoft.Faktory
{
    public class FaktoryConnection: IDisposable
    {
        private bool isDisposed;
        private readonly TcpClient tcpClient;
        private RespReader reader;
        private RespWriter respWriter;
        private StreamWriter streamWriter;

        public FaktoryConnection()
        {
            tcpClient = new TcpClient();
            Identity = ConnectionIdentity.GenerateNew();
        }

        public ConnectionIdentity Identity { get; }

        public string Password { get; set; }

        public async Task ConnectAsync(string host, int port = 7419)
        {
            await tcpClient.ConnectAsync(host, port);
            var stream = tcpClient.GetStream();
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
                Identity.PasswordHash = GetPasswordHash(Password, handshake.Nonce);
            await SendAsync(new FaktoryMessage(MessageVerb.Hello, Identity));
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
                var encoding = Encoding.UTF8;
                var bytes = encoding.GetBytes(password + nonce);
                var hash = sha.ComputeHash(bytes);
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
        }

        private async void MaintainHeartBeatAsync()
        {
            const int heartBeatPeriod = 30 * 1000; //ms
            var message = new FaktoryMessage(MessageVerb.Beat, Identity);
            while (!isDisposed)
            {
                await SendAsync(message);
                await Task.Delay(heartBeatPeriod);
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
            tcpClient.Dispose();
        }
    }
}