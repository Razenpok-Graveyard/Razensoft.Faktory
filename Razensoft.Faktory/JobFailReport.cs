using System.Runtime.Serialization;

namespace Razensoft.Faktory
{
    [DataContract]
    public class JobFailReport
    {
        [DataMember(Name = "jid")]
        public string Id { get; set; }

        [DataMember(Name = "errtype", EmitDefaultValue = false)]
        public string ErrorType { get; set; }

        [DataMember(Name = "message", EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "backtrace", EmitDefaultValue = false)]
        public string[] Backtrace { get; set; }
    }
}