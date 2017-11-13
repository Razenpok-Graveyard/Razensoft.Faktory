using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class FaktoryWorker
    {
        private readonly FaktoryWorkerConfiguration configuration;
        private readonly CancellationTokenSource fetchCancelSource = new CancellationTokenSource();
        private bool isTerminating;

        public FaktoryWorker(FaktoryWorkerConfiguration configuration) => this.configuration = configuration;

        public async Task RunAsync()
        {
            var jobConnection = await EstablishNewConnection();
            MaintainHeartBeatAsync();
            var queuesAggregated = configuration.SubscribedQueues.Aggregate((s1, s2) => $"{s1} {s2}");
            var fetchCancel = fetchCancelSource.Token;
            while (!fetchCancel.IsCancellationRequested)
            {
                await jobConnection.SendAsync(new FaktoryMessage(MessageVerb.Fetch, queuesAggregated));
                var message = await jobConnection.ReceiveAsync();
                switch (message.Verb)
                {
                    case MessageVerb.Ok:
                    case MessageVerb.None when string.IsNullOrEmpty(message.Payload):
                        continue;
                    case MessageVerb.None:
                        await ExecuteJobAsync(new Job(message.Deserialize<JobDto>()), jobConnection);
                        break;
                    default:
                        throw new Exception($"Received unexpected fetch verb {message.Verb}");
                }
            }
        }

        private async void MaintainHeartBeatAsync()
        {
            var beatConnection = await EstablishNewConnection();
            var message = new FaktoryMessage(MessageVerb.Beat, new BeatRequestDto(configuration.Identity));
            while (!isTerminating)
            {
                await beatConnection.SendAsync(message);
                var response = await beatConnection.ReceiveAsync();
                if (response.Verb != MessageVerb.Ok)
                    throw new Exception($"Received unexpected beat verb {message.Verb}");
                if (response.Payload != null)
                {
                    var dto = response.Deserialize<BeatResponseDto>();
                    switch (dto.State)
                    {
                        case BeatState.Quiet:
                            fetchCancelSource.Cancel();
                            break;
                        case BeatState.Terminate:
                            isTerminating = true;
                            await Task.Delay(configuration.TerminateTimeout);
                            // TODO: graceful failing of lingering jobs
                            Environment.Exit(0);
                            return;
                    }
                }
                await Task.Delay(configuration.HeartBeatPeriod);
            }
        }

        private async Task<FaktoryConnection> EstablishNewConnection()
        {
            var connection = new FaktoryConnection(configuration.Connection);
            await connection.ConnectAsync();
            return connection;
        }

        private async Task ExecuteJobAsync(Job job, FaktoryConnection jobConnection)
        {
            if (job == null)
                return;
            if (!configuration.Handlers.TryGetValue(job.Type, out var handler))
            {
                Console.WriteLine($"Cannot execute {job.Type}. Aborting.");
                var fail = new FailDto(job)
                {
                    ErrorMessage = $"Cannot execute {job.Type}"
                };
                await Fail(fail, jobConnection);
                return;
            }
            try
            {
                handler(job.Args);
            }
            catch (Exception e)
            {
                await Fail(new FailDto(job, e), jobConnection);
            }
            await jobConnection.SendAsync(new FaktoryMessage(MessageVerb.Ack, new AckDto(job)));
            var message = await jobConnection.ReceiveAsync();
            if (message.Verb != MessageVerb.Ok)
                throw new Exception($"Received unexpected ack response verb {message.Verb}");
        }

        private static async Task Fail(FailDto fail, FaktoryConnection jobConnection)
        {
            await jobConnection.SendAsync(new FaktoryMessage(MessageVerb.Fail, fail));
            var message = await jobConnection.ReceiveAsync();
            if (message.Verb != MessageVerb.Ok)
                throw new Exception($"Received unexpected fail response verb {message.Verb}");
        }
    }
}