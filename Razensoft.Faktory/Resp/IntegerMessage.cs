namespace Razensoft.Faktory.Resp
{
    public class IntegerMessage : SimpleSerializableMessage<long>
    {
        public const char TypeDescriptor = ':';

        public IntegerMessage() { }

        public IntegerMessage(long payload) : base(payload) { }

        public override string MessagePrefix => TypeDescriptor.ToString();

        public bool AsBool() => Payload > 0;

        protected override long Deserialize(string value) => long.Parse(value);

        protected override string Serialize(long value) => value.ToString();
    }
}