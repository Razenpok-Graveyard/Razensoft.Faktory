using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class BeatRequestDto
    {
        public BeatRequestDto(ConnectionIdentity identity) => WorkerId = identity.WorkerId;

        [DataMember(Name = "wid")]
        public string WorkerId { get; set; }
    }
}