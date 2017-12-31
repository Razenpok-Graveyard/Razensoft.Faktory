using Razensoft.Faktory.Serialization;

namespace Razensoft.Faktory
{
    public abstract class ConnectionIdentity
    {
        public abstract HandshakeResponseDto CreateHandshake();
    }
}