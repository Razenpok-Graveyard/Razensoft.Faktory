namespace Razensoft.Faktory
{
    public interface IConnectionTransportFactory
    {
        IConnectionTransport CreateTransport();
    }
}