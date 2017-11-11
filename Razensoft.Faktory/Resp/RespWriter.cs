using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Razensoft.Faktory.Resp
{
    public class RespWriter
    {
        private readonly StreamWriter streamWriter;

        internal RespWriter(StreamWriter streamWriter) => this.streamWriter = streamWriter;

        public RespWriter(Stream stream) : this(CreateStreamWriter(stream)) { }

        private static StreamWriter CreateStreamWriter(Stream stream) =>
            new StreamWriter(stream, Encoding.ASCII, 1024, true)
            {
                AutoFlush = false,
                NewLine = "\r\n"
            };

        public async Task WriteAsync(RespMessage message)
        {
            await streamWriter.WriteAsync(message.MessagePrefix);
            await message.SerializeAsync(streamWriter);
            await streamWriter.FlushAsync();
        }
    }
}