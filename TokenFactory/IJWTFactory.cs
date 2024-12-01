using MParchin.Authority.Model;
using MParchin.Authority.Schema;

namespace MParchin.Authority.TokenFactory;

public interface IJWTFactory<TJWToken, TJWTUser, TUser>
    where TJWTUser : JWTUser<TUser>, new()
    where TJWToken : JWToken
    where TUser : User, new()
{
    public Task<TJWToken> SignAsync(TUser user, TimeSpan? notBefore = null);
    public Task<TJWToken> RefreshAsync(string refreshToken);
}