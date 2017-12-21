using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Razensoft.Faktory.Resp;

namespace Razensoft.Faktory.Tests
{
    [TestFixture]
    public class RespWriterTests
    {
        private static async Task AssertWrite(string rawMessage, RespMessage message)
        {
            var stream = new MemoryStream();
            var respWriter = new RespWriter(stream);
            await respWriter.WriteAsync(message);
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var actual = await streamReader.ReadToEndAsync();
            Assert.That(actual, Is.EqualTo(rawMessage));
        }

        [Test, TestCaseSource(typeof(RespMessageTestCases), nameof(RespMessageTestCases.Enumerate))]
        public async Task Should_write_normal_messages(string name, string rawMessage, RespMessage message)
        {
            await AssertWrite(rawMessage, message);
            var stream = new MemoryStream();
            var respWriter = new RespWriter(stream);
            await respWriter.WriteAsync(message);
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var actual = await streamReader.ReadToEndAsync();
            Assert.That(actual, Is.EqualTo(rawMessage));
        }

        [Test]
        public async Task Should_write_inline_message()
        {
            // Inline message can only be sent by client, so it only works in RespWriter.
            await AssertWrite("PING\r\n", new InlineCommandMessage("PING"));
        }
    }
}