using System.Collections.Generic;
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RespMessage<T>) obj);
        }

        private bool Equals(RespMessage<T> other) => EqualityComparer<T>.Default.Equals(Payload, other.Payload);

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Payload);
    }
}