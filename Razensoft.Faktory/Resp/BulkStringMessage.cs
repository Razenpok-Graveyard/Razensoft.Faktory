using System.IO;
using System.Threading.Tasks;

namespace Razensoft.Faktory.Resp
{
    public class BulkStringMessage : BulkMessage<string>
    {
        public const char TypeDescriptor = '$';

        public BulkStringMessage() { }

        public BulkStringMessage(string payload) : base(payload) { }

        protected override int PayloadLength => Payload.Length;

        public override string MessagePrefix => TypeDescriptor.ToString();

        protected override async Task DeserializeAsync(StreamReader reader, int length)
        {
            var charBuffer = new char[length];
            await reader.ReadAsync(charBuffer, 0, length);
            // final CRLF
            await reader.ReadLineAsync();
            Payload = new string(charBuffer);
        }

        protected override async Task SerializePayload(StreamWriter writer)
        {
            await writer.WriteLineAsync(Payload);
        }
    }
}