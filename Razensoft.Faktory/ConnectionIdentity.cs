using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Razensoft.Faktory
{
    [DataContract]
    public class ConnectionIdentity
    {
        [DataMember(Name = "wid")]
        public string WorkerId { get; set; }

        [DataMember(Name = "hostname", EmitDefaultValue = false)]
        public string Hostname { get; set; }

        [DataMember(Name = "pid", EmitDefaultValue = false)]
        public int ProcessId { get; set; }

        [DataMember(Name = "labels", EmitDefaultValue = false)]
        public List<string> Labels { get; set; }

        [DataMember(Name = "pwdhash", EmitDefaultValue = false)]
        public string PasswordHash { get; set; }

        public static ConnectionIdentity GenerateNew()
        {
            return new ConnectionIdentity
            {
                WorkerId = Guid.NewGuid().ToString(),
                Hostname = ".NET Worker",
                ProcessId = Process.GetCurrentProcess().Id,
                Labels = new List<string> {"dotnet"}
            };
        }
    }
}