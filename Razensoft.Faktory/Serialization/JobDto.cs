using System;
using System.Runtime.Serialization;

namespace Razensoft.Faktory.Serialization
{
    [DataContract]
    public class JobDto
    {
        public JobDto(Job job)
        {
            JobId = job.Id;
            JobType = job.Type;
            Args = job.Args;
            Queue = job.Queue;
            Priority = job.Priority;
            ReserveFor = job.ReserveFor;
            RunAt = FormatDateTime(job.RunAt);
            Retry = job.Retry;
            Backtrace = job.Backtrace;
            CreatedAt = FormatDateTime(job.CreatedAt);
            EnqueuedAt = FormatDateTime(job.EnqueuedAt);
            Failure = job.Failure;
            Custom = job.Custom;
        }

        [DataMember(Name = "jid")]
        public string JobId { get; set; }

        [DataMember(Name = "jobtype")]
        public string JobType { get; set; }

        [DataMember(Name = "args")]
        public object[] Args { get; set; }

        [DataMember(Name = "queue", EmitDefaultValue = false)]
        public string Queue { get; set; }

        [DataMember(Name = "priority", EmitDefaultValue = false)]
        public int? Priority { get; set; }

        [DataMember(Name = "reserve_for", EmitDefaultValue = false)]
        public int? ReserveFor { get; set; }

        [DataMember(Name = "at", EmitDefaultValue = false)]
        public string RunAt { get; set; }

        [DataMember(Name = "retry", EmitDefaultValue = false)]
        public int? Retry { get; set; }

        [DataMember(Name = "backtrace", EmitDefaultValue = false)]
        public int? Backtrace { get; set; }

        [DataMember(Name = "created_at", EmitDefaultValue = false)]
        public string CreatedAt { get; set; }

        [DataMember(Name = "enqueued_at", EmitDefaultValue = false)]
        public string EnqueuedAt { get; set; }

        // What the hell is going to be here?
        [DataMember(Name = "failure", EmitDefaultValue = false)]
        public object Failure { get; set; }

        [DataMember(Name = "custom", EmitDefaultValue = false)]
        public object Custom { get; set; }

        private static string FormatDateTime(DateTime? dateTime) => dateTime?.ToString("O");
    }
}