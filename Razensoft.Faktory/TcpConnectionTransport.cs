using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Razensoft.Faktory
{
    public class TcpConnectionTransport : IConnectionTransport
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpClient tcpClient;

        public TcpConnectionTransport(IPAddress ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            tcpClient = new TcpClient();
        }

        public async Task<Stream> GetStream()
        {
            await tcpClient.ConnectAsync(ipAddress, port);
            return tcpClient.GetStream();
        }

        public void Dispose()
        {
            tcpClient.Dispose();
        }
    }
}