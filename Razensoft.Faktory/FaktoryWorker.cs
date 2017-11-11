using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class FaktoryWorker
    {
        private FaktoryConnection connection;
        private CancellationTokenSource fetchCancelSource = new CancellationTokenSource();

        // DRAFT - will move to reflection-based
        private readonly Dictionary<string, Action<object[]>> handlers = new Dictionary<string, Action<object[]>>();

        public string Password { get; set; }

        public List<string> SubscribedQueues { get; } = new List<string>();

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

        public void Register(string job, Action<object[]> handler)
        {
            handlers.Add(job, handler);
        }

        public async Task RunAsync()
        {
            var queuesAggregated = SubscribedQueues.Aggregate((s1, s2) => $"{s1} {s2}");
            var fetchCancel = fetchCancelSource.Token;
            while (!fetchCancel.IsCancellationRequested)
            {
                await connection.SendAsync(new FaktoryMessage(MessageVerb.Fetch, queuesAggregated));
                var message = await connection.ReceiveAsync();
                switch (message.Verb)
                {
                    case MessageVerb.Ok:
                        continue;
                    case MessageVerb.None:
                        await ExecuteJobAsync(new Job(message.Deserialize<JobDto>()));
                        break;
                    default:
                        throw new Exception("Whoopsie");
                }
            }
        }

        private async Task ExecuteJobAsync(Job job)
        {
            if (job == null)
                return;
            if (!handlers.TryGetValue(job.Type, out var handler))
            {
                Console.WriteLine($"Cannot execute {job.Type}. Aborting.");
                var fail = new FailDto(job)
                {
                    ErrorMessage = $"Cannot execute {job.Type}"
                };
                await Fail(fail);
                return;
            }
            try
            {
                handler(job.Args);
            }
            catch (Exception e)
            {
                await Fail(new FailDto(job, e));
            }
            await connection.SendAsync(new FaktoryMessage(MessageVerb.Ack, new AckDto(job)));
        }

        private async Task Fail(FailDto fail)
        {
            await connection.SendAsync(new FaktoryMessage(MessageVerb.Fail, fail));
            var message = await connection.ReceiveAsync();
            if (message.Verb != MessageVerb.None)
                throw new Exception("Whoopsie");
        }
    }
}