using System.Runtime.Serialization;

namespace Razensoft.Faktory
{
    [DataContract]
    public class Job
    {
        [DataMember(Name = "jid")]
        public string Id { get; set; }

        [DataMember(Name = "jobtype")]
        public string Type { get; set; }

        [DataMember(Name = "args")]
        public object[] Args { get; set; }
    }
}
