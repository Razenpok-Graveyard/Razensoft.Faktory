using System;
using System.Collections.Generic;

namespace Razensoft.Faktory
{
    public class FaktoryWorkerConfiguration : FaktoryConnectorConfiguration
    {
        private readonly Dictionary<string, Action<object[]>> handlers = new Dictionary<string, Action<object[]>>();

        private FaktoryWorkerConfiguration(WorkerConnectionIdentity identity) : base(identity) => Identity = identity;

        public FaktoryWorkerConfiguration() : this(WorkerConnectionIdentity.GenerateNew()) { }

        public WorkerConnectionIdentity Identity { get; }

        public TimeSpan HeartBeatPeriod { get; set; } = TimeSpan.FromSeconds(15);

        public TimeSpan TerminateTimeout { get; set; } = TimeSpan.FromSeconds(25);

        public List<string> SubscribedQueues { get; } = new List<string>();

        // DRAFT - will move to reflection-based
        public IReadOnlyDictionary<string, Action<object[]>> Handlers => handlers;

        public void Register(string job, Action<object[]> handler)
        {
            handlers.Add(job, handler);
        }
    }
}