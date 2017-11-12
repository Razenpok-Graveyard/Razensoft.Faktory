using System.Net;

namespace Razensoft.Faktory
{
    public abstract class FaktoryConnectorConfiguration
    {
        protected FaktoryConnectorConfiguration(ConnectionIdentity identity) =>
            Connection = new FaktoryConnectionConfiguration(identity);

        public FaktoryConnectionConfiguration Connection { get; }

        public string Password
        {
            get => Connection.Password;
            set => Connection.Password = value;
        }

        public IPAddress IpAddress
        {
            get => Connection.IpAddress;
            set => Connection.IpAddress = value;
        }

        public int Port
        {
            get => Connection.Port;
            set => Connection.Port = value;
        }
    }
}