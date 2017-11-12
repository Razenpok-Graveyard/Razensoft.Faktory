using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public class ClientConnectionIdentity : ConnectionIdentity
    {
        public override object ToHandshake() => new ClientHandshakeResponseDto(this);
    }
}