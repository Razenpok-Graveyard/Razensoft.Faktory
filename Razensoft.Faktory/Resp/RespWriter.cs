using System.IO;
using System.Threading.Tasks;

namespace Razensoft.Faktory.Resp
{
    public class RespWriter
    {
        private readonly StreamWriter streamWriter;

        public RespWriter(StreamWriter streamWriter) => this.streamWriter = streamWriter;

        public async Task WriteAsync(RespMessage message)
        {
            await streamWriter.WriteAsync(message.MessagePrefix);
            await message.SerializeAsync(streamWriter);
        }
    }
}