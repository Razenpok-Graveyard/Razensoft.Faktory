using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class WorkerHandshakeResponseDto
    {
        public WorkerHandshakeResponseDto(WorkerConnectionIdentity identity)
        {
            WorkerId = identity.WorkerId;
            Version = identity.ProtocolVersion;
            Hostname = identity.Hostname;
            ProcessId = identity.ProcessId;
            Labels = identity.Labels;
            PasswordHash = identity.PasswordHash;
        }

        [DataMember(Name = "wid")]
        public string WorkerId { get; set; }

        [DataMember(Name = "v")]
        public int Version { get; set; }

        [DataMember(Name = "hostname", EmitDefaultValue = false)]
        public string Hostname { get; set; }

        [DataMember(Name = "pid", EmitDefaultValue = false)]
        public int? ProcessId { get; set; }

        [DataMember(Name = "labels", EmitDefaultValue = false)]
        public List<string> Labels { get; set; }

        [DataMember(Name = "pwdhash", EmitDefaultValue = false)]
        public string PasswordHash { get; set; }
    }
}