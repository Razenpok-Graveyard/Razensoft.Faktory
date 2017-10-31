using System.IO;
using System.Threading.Tasks;

namespace Razensoft.Faktory.Resp
{
    public abstract class BulkMessage<T> : RespMessage<T>
    {
        protected BulkMessage() { }

        protected BulkMessage(T payload) : base(payload) { }

        protected abstract int PayloadLength { get; }

        public sealed override async Task DeserializeAsync(StreamReader reader)
        {
            var payloadLength = int.Parse(await reader.ReadLineAsync());
            if (payloadLength == -1)
            {
                Payload = default;
                return;
            }
            await DeserializePayloadAsync(reader, payloadLength);
        }

        protected abstract Task DeserializePayloadAsync(StreamReader reader, int length);

        public sealed override async Task SerializeAsync(StreamWriter writer)
        {
            await writer.WriteLineAsync(PayloadLength.ToString());
            await SerializePayloadAsync(writer);
        }

        protected abstract Task SerializePayloadAsync(StreamWriter writer);
    }
}