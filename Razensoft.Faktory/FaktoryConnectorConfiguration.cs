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
    }
}