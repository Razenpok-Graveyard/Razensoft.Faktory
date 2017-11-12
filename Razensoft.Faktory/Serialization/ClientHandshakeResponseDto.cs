using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class ClientHandshakeResponseDto
    {
        public ClientHandshakeResponseDto(ClientConnectionIdentity identity)
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