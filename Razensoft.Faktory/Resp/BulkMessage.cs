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
            await DeserializeAsync(reader, payloadLength);
        }

        protected abstract Task DeserializeAsync(StreamReader reader, int length);

        public sealed override async Task SerializeAsync(StreamWriter writer)
        {
            await writer.WriteLineAsync(PayloadLength.ToString());
            await SerializePayload(writer);
        }

        protected abstract Task SerializePayload(StreamWriter writer);
    }
}