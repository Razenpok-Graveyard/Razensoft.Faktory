namespace Razensoft.Faktory
{
    public interface IConnectionConfiguration
    {
        Password Password { get; }

        ConnectionIdentity Identity { get; }

        IConnectionTransportFactory TransportFactory { get; }
    }
}