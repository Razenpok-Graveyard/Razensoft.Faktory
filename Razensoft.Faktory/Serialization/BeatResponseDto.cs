using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class BeatResponseDto
    {
        [DataMember(Name = "state")]
        public BeatState? State { get; set; }
    }
}