namespace Razensoft.Faktory
{
    public interface IConnectionConfiguration
    {
        string Password { get; }

        /// <summary>
        /// Heart beat period in ms.
        /// </summary>
        int HeartBeatPeriod { get; }

        ConnectionIdentity Identity { get; }

        IConnectionTransportFactory TransportFactory { get; }
    }

    public interface IConnectionTransportFactory
    {
        IConnectionTransport CreateTransport();
    }
}