using MParchin.Authority.Model;

namespace MParchin.Authority.Cryptography;

public interface IHash
{
    public void SetPassword(DbUser user, string password);
    public bool VerifyPassword(DbUser user, string password);
    public void GeneratePasswordResetToken(DbUser user);
}