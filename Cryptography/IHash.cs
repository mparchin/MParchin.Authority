using MParchin.Authority.Model;

namespace MParchin.Authority.Cryptography;

public interface IHash<TDbUser>
    where TDbUser : User, IDbUser
{
    public void SetPassword(TDbUser user, string password);
    public bool VerifyPassword(TDbUser user, string password);
}