using System.IO;
using System.Threading.Tasks;

namespace Razensoft.Faktory.Resp
{
    public abstract class RespMessage
    {
        public virtual string MessagePrefix => null;

        public abstract Task DeserializeAsync(StreamReader reader);

        public abstract Task SerializeAsync(StreamWriter writer);
    }

    public abstract class RespMessage<T> : RespMessage
    {
        protected RespMessage() { }

        protected RespMessage(T payload) => Payload = payload;

        public T Payload { get; protected set; }
    }
}