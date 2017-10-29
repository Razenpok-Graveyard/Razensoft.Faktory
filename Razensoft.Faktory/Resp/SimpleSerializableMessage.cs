using System.IO;
using System.Threading.Tasks;

namespace Razensoft.Faktory.Resp
{
    public abstract class SimpleSerializableMessage<T> : RespMessage<T>
    {
        protected SimpleSerializableMessage() { }

        protected SimpleSerializableMessage(T payload) : base(payload) { }

        public sealed override async Task DeserializeAsync(StreamReader reader)
        {
            Payload = Deserialize(await reader.ReadLineAsync());
        }

        protected abstract T Deserialize(string value);

        public sealed override async Task SerializeAsync(StreamWriter writer)
        {
            await writer.WriteLineAsync(Serialize(Payload));
        }

        protected abstract string Serialize(T value);
    }
}