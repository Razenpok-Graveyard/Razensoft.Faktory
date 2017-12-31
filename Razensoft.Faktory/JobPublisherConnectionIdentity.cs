using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class JobPublisherConnectionIdentity : ConnectionIdentity
    {
        public override HandshakeResponseDto CreateHandshake() => new HandshakeResponseDto();
    }
}