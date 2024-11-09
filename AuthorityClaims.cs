using System.Security.Claims;
using MParchin.Authority.Schema;
using MParchin.Authority.TokenFactory;

namespace MParchin.Authority;

public class AuthorityClaims(ClaimsPrincipal principal, IAuthorityToken authority)
{
    private JWTUser? _user;
    public JWTUser User => _user ??= authority.GetUser(principal.ToClaimDictionary());
}