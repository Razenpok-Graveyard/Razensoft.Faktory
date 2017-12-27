using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Razensoft.Faktory.Resp;

namespace Razensoft.Faktory.Tests
{
    [TestFixture]
    public class FaktoryConnectionTests
    {
        [Test]
        public async Task SimpleConnect()
        {
            var stream = new MemoryStream();
            var respWriter = new RespWriter(stream);
            var hi = new SimpleStringMessage("HI {\"v\":\"1\"}");
            var ok = new SimpleStringMessage("OK");
            await respWriter.WriteAsync(hi);
            await respWriter.WriteAsync(ok);
            stream.Position = 0;
            var transport = new Mock<IConnectionTransport>();
            transport
                .Setup(t => t.GetStreamAsync())
                .Returns(Task.FromResult((Stream) stream));
            var transportFactory = new Mock<IConnectionTransportFactory>();
            transportFactory
                .Setup(f => f.CreateTransport())
                .Returns(transport.Object);
            var configuration = new Mock<IConnectionConfiguration>();
            configuration
                .Setup(c => c.TransportFactory)
                .Returns(transportFactory.Object);
            configuration
                .Setup(c => c.Identity)
                .Returns(WorkerConnectionIdentity.GenerateNew);
            var connection = new FaktoryConnection(configuration.Object);
            await connection.ConnectAsync();
        }
    }
}