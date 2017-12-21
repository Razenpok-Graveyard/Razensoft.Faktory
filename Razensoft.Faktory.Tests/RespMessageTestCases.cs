using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Razensoft.Faktory.Resp;

namespace Razensoft.Faktory.Tests
{
    public static class RespMessageTestCases
    {
        public static IEnumerable<TestCaseData> Enumerate()
        {
            yield return SimpleString;
            yield return Error;
            yield return Integer;
            yield return BulkString;
            yield return EmptyBulkString;
            yield return NullBulkString;
            yield return Array;
            yield return MixedArray;
            yield return NullContainingArray;
            yield return EmptyArray;
            yield return NullArray;
        }

        private static TestCaseData SimpleString => new TestCaseData(
            PrettifyName(nameof(SimpleString)),
            "+OK\r\n",
            new SimpleStringMessage("OK"));

        private static TestCaseData Error => new TestCaseData(
            PrettifyName(nameof(Error)),
            "-ERR unknown command 'foobar'\r\n",
            new ErrorMessage("ERR unknown command 'foobar'"));

        private static TestCaseData Integer => new TestCaseData(
            PrettifyName(nameof(Integer)),
            ":1000\r\n",
            new IntegerMessage(1000));

        private static TestCaseData BulkString => new TestCaseData(
            PrettifyName(nameof(BulkString)),
            "$6\r\nfoobar\r\n",
            new BulkStringMessage("foobar"));

        private static TestCaseData EmptyBulkString => new TestCaseData(
            PrettifyName(nameof(EmptyBulkString)),
            "$0\r\n\r\n",
            new BulkStringMessage(string.Empty));

        private static TestCaseData NullBulkString => new TestCaseData(
            PrettifyName(nameof(NullBulkString)),
            "$-1\r\n",
            new BulkStringMessage(null));

        private static TestCaseData Array => new TestCaseData(
            PrettifyName(nameof(Array)),
            "*2\r\n$3\r\nfoo\r\n$3\r\nbar\r\n",
            new ArrayMessage(new RespMessage[]
            {
                new BulkStringMessage("foo"),
                new BulkStringMessage("bar")
            }));

        private static TestCaseData MixedArray => new TestCaseData(
            PrettifyName(nameof(MixedArray)),
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
                    new ErrorMessage("Bar")
                })
            }));

        private static TestCaseData NullContainingArray => new TestCaseData(
            PrettifyName(nameof(NullContainingArray)),
            "*3\r\n$3\r\nfoo\r\n$-1\r\n$3\r\nbar\r\n",
            new ArrayMessage(new RespMessage[]
            {
                new BulkStringMessage("foo"),
                new BulkStringMessage(null),
                new BulkStringMessage("bar")
            }));

        private static TestCaseData EmptyArray => new TestCaseData(
            PrettifyName(nameof(EmptyArray)),
            "*0\r\n",
            new ArrayMessage(new RespMessage[0]));

        private static TestCaseData NullArray => new TestCaseData(
            PrettifyName(nameof(NullArray)),
            "*-1\r\n",
            new ArrayMessage(null));

        private static string PrettifyName(string name)
        {
            return Regex.Replace(name, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }
    }
}