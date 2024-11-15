using MParchin.Authority.Schema;

namespace MParchin.Authority.TokenFactory;

public interface IAuthorityToken
{
    public JWTUser GetUser(Dictionary<string, string> claims);
    public Dictionary<string, string> GetClaims(JWTUser user);
    public void VerifyJWTToken(JWTUser user);
}