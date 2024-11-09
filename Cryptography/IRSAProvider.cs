using System.Security.Cryptography;

namespace MParchin.Authority.Cryptography;

public interface IRSAProvider : IDisposable
{
    public RSA Key { get; }
}