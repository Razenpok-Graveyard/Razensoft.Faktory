namespace Razensoft.Faktory
{
    public class FaktoryClientConfiguration : FaktoryConnectorConfiguration
    {
        private FaktoryClientConfiguration(ClientConnectionIdentity identity) : base(identity) => Identity = identity;

        public FaktoryClientConfiguration() : this(new ClientConnectionIdentity()) { }

        public ClientConnectionIdentity Identity { get; }
    }
}