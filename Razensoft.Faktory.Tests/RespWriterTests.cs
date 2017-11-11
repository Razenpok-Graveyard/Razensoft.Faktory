using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Razensoft.Faktory.Resp;

namespace Razensoft.Faktory.Tests
{
    [TestFixture]
    public class RespWriterTests
    {
        private static async Task AssertWrite(RespMessage input, string expected)
        {
            var stream = new MemoryStream();
            var respWriter = new RespWriter(stream);
            await respWriter.WriteAsync(input);
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var actual = await streamReader.ReadToEndAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task WriteSimpleString()
        {
            await AssertWrite(
                new SimpleStringMessage("OK"),
                "+OK\r\n");
        }

        [Test]
        public async Task WriteError()
        {
            await AssertWrite(
                new ErrorMessage("ERR unknown command 'foobar'"),
                "-ERR unknown command 'foobar'\r\n");
        }

        [Test]
        public async Task WriteInteger()
        {
            await AssertWrite(
                new IntegerMessage(1000),
                ":1000\r\n");
        }

        [Test]
        public async Task WriteBulkString()
        {
            await AssertWrite(
                new BulkStringMessage("foobar"),
                "$6\r\nfoobar\r\n");
        }

        [Test]
        public async Task WriteEmptyBulkString()
        {
            await AssertWrite(
                new BulkStringMessage(string.Empty),
                "$0\r\n\r\n");
        }

        [Test]
        public async Task WriteNullBulkString()
        {
            await AssertWrite(
                new BulkStringMessage(null),
                "$-1\r\n");
        }

        [Test]
        public async Task WriteArray()
        {
            await AssertWrite(
                new ArrayMessage(new RespMessage[]
                {
                    new BulkStringMessage("foo"),
                    new BulkStringMessage("bar")
                }),
                "*2\r\n$3\r\nfoo\r\n$3\r\nbar\r\n");
        }

        [Test]
        public async Task WriteEmptyArray()
        {
            await AssertWrite(
                new ArrayMessage(new RespMessage[0]),
                "*0\r\n");
        }

        [Test]
        public async Task WriteMixedArray()
        {
            await AssertWrite(
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
                }),
                "*2\r\n*3\r\n:1\r\n:2\r\n:3\r\n*2\r\n+Foo\r\n-Bar\r\n");
        }

        [Test]
        public async Task WriteNullArray()
        {
            await AssertWrite(
                new ArrayMessage(null),
                "*-1\r\n");
        }

        [Test]
        public async Task WriteNullContainingArray()
        {
            await AssertWrite(
                new ArrayMessage(new RespMessage[]
                {
                    new BulkStringMessage("foo"),
                    new BulkStringMessage(null),
                    new BulkStringMessage("bar"),
                }),
                "*3\r\n$3\r\nfoo\r\n$-1\r\n$3\r\nbar\r\n");
        }

        [Test]
        public async Task WriteInlineCommand()
        {
            await AssertWrite(
                new InlineCommandMessage("PING"),
                "PING\r\n");
        }
    }
}