using System.Security.Cryptography;

namespace MParchin.Authority.Cryptography;

public class RSAProvider : IRSAProvider
{
    public RSACryptoServiceProvider Provider { get; }
    public RSAProvider(string path)
    {
        Provider = new();
        if (File.Exists(path))
            Provider.ImportFromPem(File.ReadAllText(path).ToCharArray());
    }

    public void Dispose()
    {
        Provider.Dispose();
        GC.SuppressFinalize(this);
    }
}