using System.IO;
using System.Threading.Tasks;

namespace Razensoft.Faktory.Resp
{
    public class InlineCommandMessage : RespMessage<string>
    {
        public InlineCommandMessage() { }

        public InlineCommandMessage(string payload) : base(payload) { }

        public override string MessagePrefix => string.Empty;

        public override async Task DeserializeAsync(StreamReader reader)
        {
            Payload = await reader.ReadLineAsync();
        }

        public override async Task SerializeAsync(StreamWriter writer)
        {
            await writer.WriteLineAsync(Payload);
        }
    }
}