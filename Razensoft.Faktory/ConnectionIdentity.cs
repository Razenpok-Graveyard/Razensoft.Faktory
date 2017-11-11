using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Razensoft.Faktory
{
    public class ConnectionIdentity
    {
        public string WorkerId { get; set; }

        public string Hostname { get; set; }

        public int ProcessId { get; set; }

        public List<string> Labels { get; set; }

        public string PasswordHash { get; set; }

        public static ConnectionIdentity GenerateNew() => new ConnectionIdentity
        {
            WorkerId = Guid.NewGuid().ToString(),
            Hostname = ".NET Worker",
            ProcessId = Process.GetCurrentProcess().Id,
            Labels = new List<string> {"dotnet"}
        };
    }
}