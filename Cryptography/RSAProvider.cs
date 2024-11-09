using System.Security.Cryptography;

namespace MParchin.Authority.Cryptography;

public class RSAProvider : IRSAProvider
{
    public RSA Key { get; }
    public RSAProvider(string path)
    {
        Key = RSA.Create();
        if (File.Exists(path))
            Key.ImportFromPem(File.ReadAllText(path).ToCharArray());
    }

    public void Dispose()
    {
        Key.Dispose();
        GC.SuppressFinalize(this);
    }
}