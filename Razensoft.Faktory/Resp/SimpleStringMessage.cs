namespace Razensoft.Faktory.Resp
{
    public class SimpleStringMessage : SimpleSerializableMessage<string>
    {
        public const char TypeDescriptor = '+';

        public SimpleStringMessage() { }

        public SimpleStringMessage(string payload) : base(payload) { }

        public override string MessagePrefix => TypeDescriptor.ToString();

        protected override string Deserialize(string value) => value;

        protected override string Serialize(string value) => value;
    }
}