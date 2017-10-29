namespace Razensoft.Faktory.Resp
{
    public class ErrorMessage : SimpleSerializableMessage<string>
    {
        public const char TypeDescriptor = '-';

        public ErrorMessage() { }

        public ErrorMessage(string payload) : base(payload) { }

        public override string MessagePrefix => TypeDescriptor.ToString();

        public (string type, string message) ParseByConvention()
        {
            // TODO: validation
            var parts = Payload.Split(' ');
            return (parts[0], parts[1]);
        }

        protected override string Deserialize(string value) => value;

        protected override string Serialize(string value) => value;
    }
}