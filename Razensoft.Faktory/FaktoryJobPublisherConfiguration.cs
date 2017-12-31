namespace Razensoft.Faktory
{
    public class FaktoryJobPublisherConfiguration : FaktoryConnectorConfiguration
    {
        private FaktoryJobPublisherConfiguration(JobPublisherConnectionIdentity identity) : base(identity) => Identity = identity;

        public FaktoryJobPublisherConfiguration() : this(new JobPublisherConnectionIdentity()) { }

        public JobPublisherConnectionIdentity Identity { get; }
    }
}