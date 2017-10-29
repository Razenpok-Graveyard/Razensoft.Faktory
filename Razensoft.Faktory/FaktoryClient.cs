using System;
using System.Threading.Tasks;

namespace Razensoft.Faktory
{
    public class FaktoryClient
    {
        private FaktoryConnection connection;

        public async Task ConnectAsync(string host, int port = 7419)
        {
            connection = new FaktoryConnection();
            await connection.ConnectAsync(host, port);
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