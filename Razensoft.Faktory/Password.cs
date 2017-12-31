using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Razensoft.Faktory
{
    public struct Password
    {
        private readonly string password;

        public Password(string password) => this.password = password;

        public string GetHash(string nonce, int iterations)
        {
            var data = Encoding.ASCII.GetBytes(password + nonce);
            var sha = SHA256.Create();
            for (var i = 0; i < iterations; i++)
                data = sha.ComputeHash(data);
            sha.Dispose();
            return string.Concat(data.Select(b => b.ToString("x2")));
        }

        public static implicit operator Password(string value) => new Password(value);
    }
}