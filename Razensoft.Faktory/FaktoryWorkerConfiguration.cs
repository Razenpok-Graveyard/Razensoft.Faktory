using System;
using System.Collections.Generic;
using System.Net;

namespace Razensoft.Faktory
{
    public class FaktoryWorkerConfiguration
    {
        private readonly Dictionary<string, Action<object[]>> handlers = new Dictionary<string, Action<object[]>>();

        public FaktoryConnectionConfiguration Connection { get; } = new FaktoryConnectionConfiguration();

        public IPAddress IpAddress
        {
            get => Connection.IpAddress;
            set => Connection.IpAddress = value;
        }

        public int Port
        {
            get => Connection.Port;
            set => Connection.Port = value;
        }

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