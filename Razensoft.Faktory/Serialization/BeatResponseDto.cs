using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class BeatResponseDto
    {
        [DataMember(Name = "signal")]
        public BeatState? WorkerId { get; set; }
    }
}