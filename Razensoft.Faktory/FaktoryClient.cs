using System;
using System.Net;
using System.Threading.Tasks;

namespace Razensoft.Faktory
{
    public class FaktoryClient
    {
        private FaktoryConnection connection;

        public string Password { get; set; }

        public async Task ConnectAsync(string host, int port = 7419)
        {
            var configuration = new FaktoryConnectionConfiguration
            {
                IpAddress = IPAddress.Parse(host),
                Port = port,
                Password = Password
            };
            connection = new FaktoryConnection(configuration);
            await connection.ConnectAsync();
        }

        public async Task PublishAsync(Job job)
        {
            await connection.SendAsync(new FaktoryMessage(MessageVerb.Push, job));
            var message = await connection.ReceiveAsync();
            if (message.Verb != MessageVerb.Ok)
                throw new Exception("Whoopsie");
        }
    }
}