using System;
using Newtonsoft.Json;
using Razensoft.Faktory.Resp;

namespace Razensoft.Faktory
{
    public struct FaktoryMessage
    {
        public MessageVerb Verb { get; }
        public string Payload { get; }

        public FaktoryMessage(MessageVerb verb, string payload)
        {
            Verb = verb;
            Payload = payload;
        }

        public FaktoryMessage(MessageVerb verb, object payload): this(verb, JsonConvert.SerializeObject(payload)) { }

        public FaktoryMessage(SimpleStringMessage message)
        {
            var line = message.Payload;
            var spaceIndex = line.IndexOf(' ');
            var hasPayload = spaceIndex != -1;
            Verb = hasPayload ? ParseVerb(line.Substring(0, spaceIndex)) : ParseVerb(line);
            Payload = hasPayload ? line.Substring(spaceIndex) : null;
        }

        public FaktoryMessage(BulkStringMessage bulkString) : this(MessageVerb.None, bulkString.Payload) { }

        private static MessageVerb ParseVerb(string value)
        {
            return Enum.TryParse(value, true, out MessageVerb verb) ? verb : MessageVerb.Unknown;
        }

        public T ParseJsonPayload<T>()
        {
            return JsonConvert.DeserializeObject<T>(Payload);
        }
    }
}