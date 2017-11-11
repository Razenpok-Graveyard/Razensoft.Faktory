using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class AckDto
    {
        public AckDto(Job job) => JobId = job.Id;

        [DataMember(Name = "jid")]
        public string JobId { get; set; }
    }
}