using System;
using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class FailDto
    {
        public FailDto(Job job) => JobId = job.Id;

        public FailDto(Job job, Exception exception) : this(job)
        {
            ErrorType = exception.GetType().Namespace;
            ErrorMessage = exception.Message;
            Backtrace = exception.StackTrace
                .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        }

        [DataMember(Name = "jid")]
        public string JobId { get; set; }

        [DataMember(Name = "errtype", EmitDefaultValue = false)]
        public string ErrorType { get; set; }

        [DataMember(Name = "message", EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "backtrace", EmitDefaultValue = false)]
        public string[] Backtrace { get; set; }
    }
}