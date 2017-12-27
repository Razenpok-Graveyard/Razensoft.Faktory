using System;
using System.Net;

namespace Razensoft.Faktory
{
    public class FaktoryEndPointProvider
    {
        private readonly Func<IPEndPoint> provider;

        private FaktoryEndPointProvider(Func<IPEndPoint> provider)
        {
            this.provider = provider;
        }

        public IPEndPoint GetEndPoint()
        {
            return provider?.Invoke();
        }

        public static FaktoryEndPointProvider FromIpAddressAndPort(IPAddress ipAddress, int port = 7419)
        {
            return new FaktoryEndPointProvider(() => new IPEndPoint(ipAddress, port));
        }

        public static FaktoryEndPointProvider FromEnvironmentVariables()
        {
            return new FaktoryEndPointProvider(GetEndPointFromEnvironmentVariables);
        }

        private static IPEndPoint GetEndPointFromEnvironmentVariables()
        {
            const string defaultProvider = "FAKTORY_URL";
            const string providerLocator = "FAKTORY_PROVIDER";
            var provider = Environment.GetEnvironmentVariable(providerLocator) ?? defaultProvider;
            var url = Environment.GetEnvironmentVariable(provider);
            var uri = new Uri(url);
            // TODO: Add validation
            return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port);
        }
    }
}