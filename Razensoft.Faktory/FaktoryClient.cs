using System;
using System.Threading.Tasks;
using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class FaktoryClient
    {
        private readonly FaktoryWorkerConfiguration configuration;
        private FaktoryConnection connection;

        public FaktoryClient(FaktoryWorkerConfiguration configuration) => this.configuration = configuration;

        public async Task ConnectAsync()
        {
            connection = new FaktoryConnection(configuration.Connection);
            await connection.ConnectAsync();
        }

        public async Task PublishAsync(Job job)
        {
            await connection.SendAsync(new FaktoryMessage(MessageVerb.Push, new JobDto(job)));
            var message = await connection.ReceiveAsync();
            if (message.Verb != MessageVerb.Ok)
                throw new Exception("Whoopsie");
        }
    }
}