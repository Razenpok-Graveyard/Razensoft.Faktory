using System.Runtime.Serialization;

namespace Razensoft.Faktory
{
    [DataContract]
    public class FaktoryHandshake
    {
        [DataMember(Name = "v")]
        public string Version { get; set; }

        [DataMember(Name = "s")]
        public string Nonce { get; set; }
    }
}