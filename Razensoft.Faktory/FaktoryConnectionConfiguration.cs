using System.Net;

namespace Razensoft.Faktory
{
    public class FaktoryConnectionConfiguration: IConnectionConfiguration
    {
        private readonly TcpConnectionFactory transportFactory = new TcpConnectionFactory();

        public IPAddress IpAddress
        {
            get => transportFactory.IpAddress;
            set => transportFactory.IpAddress = value;
        }

        public int Port
        {
            get => transportFactory.Port;
            set => transportFactory.Port = value;
        }

        public string Password { get; set; }

        public int HeartBeatPeriod { get; set; } = 30 * 1000;

        public ConnectionIdentity Identity { get; } = ConnectionIdentity.GenerateNew();

        IConnectionTransportFactory IConnectionConfiguration.TransportFactory => transportFactory;

        private class TcpConnectionFactory : IConnectionTransportFactory
        {
            public IPAddress IpAddress { get; set; } = IPAddress.Any;

            public int Port { get; set; } = 7419;

            public IConnectionTransport CreateTransport()
            {
                return new TcpConnectionTransport(IpAddress, Port);
            }
        }

    }
}