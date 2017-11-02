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
            var streamWriter = new StreamWriter(stream);
            var respWriter = new RespWriter(streamWriter);
            await respWriter.WriteAsync(input);
            await streamWriter.FlushAsync();
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var actual = await streamReader.ReadToEndAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task TestSimpleStringWrite()
        {
            await AssertWrite(
                new SimpleStringMessage("OK"),
                "+OK\r\n");
        }

        [Test]
        public async Task TestErrorWrite()
        {
            await AssertWrite(
                new ErrorMessage("ERR unknown command 'foobar'"),
                "-ERR unknown command 'foobar'\r\n");
        }

        [Test]
        public async Task TestIntegerWrite()
        {
            await AssertWrite(
                new IntegerMessage(1000),
                ":1000\r\n");
        }

        [Test]
        public async Task TestBulkStringWrite()
        {
            await AssertWrite(
                new BulkStringMessage("foobar"),
                "$6\r\nfoobar\r\n");
        }

        [Test]
        public async Task TestEmptyBulkStringWrite()
        {
            await AssertWrite(
                new BulkStringMessage(string.Empty),
                "$0\r\n\r\n");
        }

        [Test]
        public async Task TestNullBulkStringWrite()
        {
            await AssertWrite(
                new BulkStringMessage(null),
                "$-1\r\n");
        }

        [Test]
        public async Task TestArrayWrite()
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
        public async Task TestEmptyArrayWrite()
        {
            await AssertWrite(
                new ArrayMessage(new RespMessage[0]),
                "*0\r\n");
        }

        [Test]
        public async Task TestMixedArrayWrite()
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
        public async Task TestNullArrayWrite()
        {
            await AssertWrite(
                new ArrayMessage(null),
                "*-1\r\n");
        }

        [Test]
        public async Task TestNullContainingArrayWrite()
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
        public async Task TestInlineCommandWrite()
        {
            await AssertWrite(
                new InlineCommandMessage("PING"),
                "PING\r\n");
        }
    }
}