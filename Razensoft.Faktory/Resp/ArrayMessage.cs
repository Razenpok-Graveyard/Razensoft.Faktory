using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Razensoft.Faktory.Resp
{
    public class ArrayMessage : BulkMessage<RespMessage[]>
    {
        public const char TypeDescriptor = '*';

        public ArrayMessage() { }

        public ArrayMessage(RespMessage[] payload) : base(payload) { }

        protected override int PayloadLength => Payload.Length;

        public override string MessagePrefix => TypeDescriptor.ToString();

        protected override async Task DeserializePayloadAsync(StreamReader reader, int length)
        {
            Payload = new RespMessage[length];
            var respReader = new RespReader(reader);
            for (var i = 0; i < length; i++)
                Payload[i] = await respReader.ReadAsync();
        }

        protected override async Task SerializePayloadAsync(StreamWriter writer)
        {
            var respWriter = new RespWriter(writer);
            foreach (var message in Payload)
                await respWriter.WriteAsync(message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ArrayMessage) obj);
        }

        private bool Equals(ArrayMessage other) =>
            ReferenceEquals(null, other.Payload) && ReferenceEquals(null, Payload)
            || ReferenceEquals(Payload, other.Payload)
            || Payload.SequenceEqual(other.Payload);

        public override int GetHashCode() => Payload.GetHashCode();
    }
}