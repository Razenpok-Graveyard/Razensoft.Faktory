using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class JobPublisherConnectionIdentity : ConnectionIdentity
    {
        public override object ToHandshake() => new JobPublisherHandshakeResponseDto(this);
    }
}