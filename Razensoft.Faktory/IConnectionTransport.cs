using System;
using System.IO;
using System.Threading.Tasks;

namespace Razensoft.Faktory
{
    public interface IConnectionTransport : IDisposable
    {
        Task<Stream> GetStream();
    }
}