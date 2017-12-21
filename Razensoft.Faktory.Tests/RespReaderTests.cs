using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Razensoft.Faktory.Resp;

namespace Razensoft.Faktory.Tests
{
    [TestFixture]
    public class RespReaderTests
    {
        private static Encoding Encoding => Encoding.ASCII;

        [Test, TestCaseSource(typeof(RespMessageTestCases), nameof(RespMessageTestCases.Enumerate))]
        public async Task Should_read_normal_messages(string name, string rawMessage, RespMessage message)
        {
            var stream = new MemoryStream(Encoding.GetBytes(rawMessage));
            var reader = new RespReader(stream);
            var actual = await reader.ReadAsync();
            Assert.That(actual, Is.EqualTo(message));
        }
    }
}