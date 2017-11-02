using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Razensoft.Faktory
{
    public class FaktoryWorker
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

        public List<string> SubscribedQueues { get; } = new List<string>();

        // DRAFT - will move to reflection-based
        private Dictionary<string, Action<object[]>> handlers = new Dictionary<string, Action<object[]>>();

        public void Register(string job, Action<object[]> handler)
        {
            handlers.Add(job, handler);
        }

        public async Task RunAsync()
        {
            var queuesAggregated = SubscribedQueues.Aggregate((s1, s2) => $"{s1} {s2}");
            while (true)
            {
                await connection.SendAsync(new FaktoryMessage(MessageVerb.Fetch, queuesAggregated));
                var message = await connection.ReceiveAsync();
                switch (message.Verb)
                {
                    case MessageVerb.Ok:
                        continue;
                    case MessageVerb.None:
                        await ExecuteJobAsync(message.Deserialize<Job>());
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
                await Fail(job, errorMessage: $"Cannot execute {job.Type}");
                return;
            }
            try
            {
                handler(job.Args);
            }
            catch (Exception e)
            {
                await Fail(job, e.GetType().Name, e.Message,
                    e.StackTrace.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries));
            }
            await connection.SendAsync(new FaktoryMessage(MessageVerb.Ack, new { jid = job.Id}));
        }

        private async Task Fail(Job job, string errorType = null, string errorMessage = null, string[] backtrace = null)
        {
            var report = new JobFailReport
            {
                Id = job.Id,
                ErrorType = errorType,
                ErrorMessage = errorMessage,
                Backtrace = backtrace
            };
            await connection.SendAsync(new FaktoryMessage(MessageVerb.Fail, report));
            var message = await connection.ReceiveAsync();
            if (message.Verb != MessageVerb.None)
                throw new Exception("Whoopsie");
        }
    }
}