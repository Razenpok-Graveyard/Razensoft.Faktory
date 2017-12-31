using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class WorkerHandshakeResponseDto: HandshakeResponseDto
    {
        public WorkerHandshakeResponseDto(WorkerConnectionIdentity identity)
        {
            WorkerId = identity.WorkerId;
            Hostname = identity.Hostname;
            ProcessId = identity.ProcessId;
            Labels = identity.Labels;
        }

        [DataMember(Name = "wid")]
        public string WorkerId { get; set; }

        [DataMember(Name = "hostname", EmitDefaultValue = false)]
        public string Hostname { get; set; }

        [DataMember(Name = "pid", EmitDefaultValue = false)]
        public int? ProcessId { get; set; }

        [DataMember(Name = "labels", EmitDefaultValue = false)]
        public List<string> Labels { get; set; }
    }

    [DataContract]
    public class HandshakeResponseDto
    {
        [DataMember(Name = "v")]
        public int Version { get; set; }

        [DataMember(Name = "pwdhash", EmitDefaultValue = false)]
        public string PasswordHash { get; set; }
    }
}