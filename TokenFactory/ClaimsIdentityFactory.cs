using System.Security.Principal;
using JWT.Extensions.AspNetCore;
using JWT.Extensions.AspNetCore.Factories;
using Microsoft.Extensions.Options;

namespace MParchin.Authority.TokenFactory;

public class ClaimsIdentityFactory(IOptionsMonitor<JwtAuthenticationOptions> options, IAuthorityToken authorityToken) :
    JWT.Extensions.AspNetCore.Factories.ClaimsIdentityFactory(options), IIdentityFactory
{
    public new IIdentity CreateIdentity(Type type, object payload)
    {
        var identity = base.CreateIdentity(type, payload);
        var claims = ReadClaims(type, payload);

        authorityToken.VerifyJWTToken(authorityToken.GetUser(claims.ToDictionary(claim => claim.Type, claim => claim.Value)));

        return identity;
    }
}