using System;
using System.Collections.Generic;
using System.Diagnostics;
using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class WorkerConnectionIdentity : ConnectionIdentity
    {
        public string WorkerId { get; set; }

        public string Hostname { get; set; }

        public int ProcessId { get; set; }

        public List<string> Labels { get; set; }

        public static WorkerConnectionIdentity GenerateNew() => new WorkerConnectionIdentity
        {
            WorkerId = Guid.NewGuid().ToString(),
            Hostname = ".NET Worker",
            ProcessId = Process.GetCurrentProcess().Id,
            Labels = new List<string> {"dotnet"}
        };

        public override HandshakeResponseDto CreateHandshake() => new WorkerHandshakeResponseDto(this);
    }
}