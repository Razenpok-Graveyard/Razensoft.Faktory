using System.IO;
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

        protected override async Task DeserializeAsync(StreamReader reader, int length)
        {
            Payload = new RespMessage[length];
            var respReader = new RespReader(reader);
            for (var i = 0; i < length; i++)
                Payload[i] = await respReader.ReadAsync();
        }

        protected override async Task SerializePayload(StreamWriter writer)
        {
            var respWriter = new RespWriter(writer);
            foreach (var message in Payload)
                await respWriter.WriteAsync(message);
        }
    }
}