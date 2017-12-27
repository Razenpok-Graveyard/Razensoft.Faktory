using System;
using System.Net;

namespace Razensoft.Faktory
{
    public class FaktoryConnectionConfiguration : IConnectionConfiguration
    {
        private readonly Lazy<IPEndPoint> endPoint;

        public FaktoryConnectionConfiguration(ConnectionIdentity identity)
        {
            Identity = identity;
            endPoint = new Lazy<IPEndPoint>(GetEndpoint);
        }

        public FaktoryEndPointProvider EndPointProvider { get; set; } =
            FaktoryEndPointProvider.FromEnvironmentVariables();

        public string Password { get; set; }

        public ConnectionIdentity Identity { get; }

        private IPEndPoint GetEndpoint()
        {
            return EndPointProvider.GetEndPoint();
        }

        IConnectionTransportFactory IConnectionConfiguration.TransportFactory =>
            new TcpConnectionFactory(endPoint.Value);

        private class TcpConnectionFactory : IConnectionTransportFactory
        {
            public TcpConnectionFactory(IPEndPoint endPoint)
            {
                EndPoint = endPoint;
            }

            private IPEndPoint EndPoint { get; }

            public IConnectionTransport CreateTransport() =>
                new TcpConnectionTransport(EndPoint.Address, EndPoint.Port);
        }
    }
}