using System.Security.Cryptography;

namespace MParchin.Authority.Cryptography;

public interface IRSAProvider : IDisposable
{
    public RSACryptoServiceProvider Provider { get; }
}