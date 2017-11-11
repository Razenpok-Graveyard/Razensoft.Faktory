namespace Razensoft.Faktory
{
    public interface IConnectionConfiguration
    {
        string Password { get; }

        ConnectionIdentity Identity { get; }

        IConnectionTransportFactory TransportFactory { get; }
    }

    public interface IConnectionTransportFactory
    {
        IConnectionTransport CreateTransport();
    }
}