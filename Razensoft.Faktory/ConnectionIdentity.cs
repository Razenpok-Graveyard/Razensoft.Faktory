namespace Razensoft.Faktory
{
    public abstract class ConnectionIdentity
    {
        public int ProtocolVersion { get; } = 2;

        public string PasswordHash { get; set; }

        public abstract object ToHandshake();
    }
}