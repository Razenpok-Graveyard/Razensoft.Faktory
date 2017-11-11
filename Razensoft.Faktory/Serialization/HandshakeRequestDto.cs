using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class HandshakeRequestDto
    {
        [DataMember(Name = "v")]
        public int Version { get; set; }

        [DataMember(Name = "s")]
        public string Nonce { get; set; }

        [DataMember(Name = "i")]
        public int HashIterations { get; set; }
    }
}