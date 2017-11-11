using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class HandshakeRequestDto
    {
        [DataMember(Name = "v")]
        public string Version { get; set; }

        [DataMember(Name = "s")]
        public string Nonce { get; set; }
    }
}