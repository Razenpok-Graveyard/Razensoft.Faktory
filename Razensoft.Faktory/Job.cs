using System;
using System.Globalization;
using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class Job
    {
        public Job() => Id = Guid.NewGuid().ToString();

        public Job(JobDto dto)
        {
            Id = dto.JobId;
            Type = dto.JobType;
            Args = dto.Args;
            Queue = dto.Queue;
            Priority = dto.Priority;
            ReserveFor = dto.ReserveFor;
            RunAt = ParseDateTime(dto.RunAt);
            Retry = dto.Retry;
            Backtrace = dto.Backtrace;
            CreatedAt = ParseDateTime(dto.CreatedAt);
            EnqueuedAt = ParseDateTime(dto.EnqueuedAt);
            Failure = dto.Failure;
            Custom = dto.Custom;
        }

        public string Id { get; set; }

        public string Type { get; set; }

        public object[] Args { get; set; }

        public string Queue { get; set; }

        public int? Priority { get; set; }

        public int? ReserveFor { get; set; }

        public DateTime? RunAt { get; set; }

        public int? Retry { get; set; }

        public int? Backtrace { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? EnqueuedAt { get; set; }

        public object Failure { get; set; }

        public object Custom { get; set; }

        private static DateTime? ParseDateTime(string value) => DateTime.TryParseExact(value, "O",
            CultureInfo.InvariantCulture, DateTimeStyles.None,
            out var result)
            ? result
            : (DateTime?) null;
    }
}