using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class JobPublisherHandshakeResponseDto
    {
        public JobPublisherHandshakeResponseDto(JobPublisherConnectionIdentity identity)
        {
            Version = identity.ProtocolVersion;
            PasswordHash = identity.PasswordHash;
        }

        [DataMember(Name = "v")]
        public int Version { get; set; }

        [DataMember(Name = "pwdhash", EmitDefaultValue = false)]
        public string PasswordHash { get; set; }
    }
}