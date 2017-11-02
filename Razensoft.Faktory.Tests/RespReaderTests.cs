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

        private static RespReader ArrangeTestCase(string input)
        {
            var stream = new MemoryStream(Encoding.GetBytes(input));
            var reader = new StreamReader(stream);
            return new RespReader(reader);
        }

        private static async Task AssertRead(string input, RespMessage expected)
        {
            var reader = ArrangeTestCase(input);
            var actual = await reader.ReadAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task ReadSimpleString()
        {
            await AssertRead(
                "+OK\r\n",
                new SimpleStringMessage("OK"));
        }

        [Test]
        public async Task ReadError()
        {
            await AssertRead(
                "-ERR unknown command 'foobar'\r\n",
                new ErrorMessage("ERR unknown command 'foobar'"));
        }

        [Test]
        public async Task ReadInteger()
        {
            await AssertRead(
                ":1000\r\n",
                new IntegerMessage(1000));
        }

        [Test]
        public async Task ReadBulkString()
        {
            await AssertRead(
                "$6\r\nfoobar\r\n",
                new BulkStringMessage("foobar"));
        }

        [Test]
        public async Task ReadEmptyBulkString()
        {
            await AssertRead(
                "$0\r\n\r\n",
                new BulkStringMessage(string.Empty));
        }

        [Test]
        public async Task ReadNullBulkString()
        {
            await AssertRead(
                "$-1\r\n",
                new BulkStringMessage(null));
        }

        [Test]
        public async Task ReadArray()
        {
            await AssertRead(
                "*2\r\n$3\r\nfoo\r\n$3\r\nbar\r\n",
                new ArrayMessage(new RespMessage[]
                {
                    new BulkStringMessage("foo"),
                    new BulkStringMessage("bar")
                }));
        }

        [Test]
        public async Task ReadEmptyArray()
        {
            await AssertRead(
                "*0\r\n",
                new ArrayMessage(new RespMessage[0]));
        }

        [Test]
        public async Task ReadMixedArray()
        {
            await AssertRead(
                "*2\r\n*3\r\n:1\r\n:2\r\n:3\r\n*2\r\n+Foo\r\n-Bar\r\n",
                new ArrayMessage(new RespMessage[]
                {
                    new ArrayMessage(new RespMessage[]
                    {
                        new IntegerMessage(1),
                        new IntegerMessage(2),
                        new IntegerMessage(3)
                    }),
                    new ArrayMessage(new RespMessage[]
                    {
                        new SimpleStringMessage("Foo"),
                        new ErrorMessage("Bar"),
                    }),
                }));
        }

        [Test]
        public async Task ReadNullArray()
        {
            await AssertRead(
                "*-1\r\n",
                new ArrayMessage(null));
        }

        [Test]
        public async Task ReadNullContainingArray()
        {
            await AssertRead(
                "*3\r\n$3\r\nfoo\r\n$-1\r\n$3\r\nbar\r\n",
                new ArrayMessage(new RespMessage[]
                {
                    new BulkStringMessage("foo"),
                    new BulkStringMessage(null),
                    new BulkStringMessage("bar"),
                }));
        }
    }
}
