using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MParchin.Authority.Exceptions;
using MParchin.Authority.Model;
using MParchin.Authority.Schema;

namespace MParchin.Authority;

public class JWTAuthenticationHandler<TJWTUser, TUser>(IOptionsMonitor<JWTAuthenticationOptions> options,
    ILoggerFactory logger, UrlEncoder encoder, IAuthority<TJWTUser, TUser> authority)
        : AuthenticationHandler<JWTAuthenticationOptions>(options, logger, encoder)
    where TJWTUser : JWTUser<TUser>, new()
    where TUser : User, new()
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var results = await Task.WhenAll(Request.Headers.Authorization.Select(async auth =>
        {
            var bearer = auth?.Split(' ');
            if (bearer?.Length != 2)
                return AuthenticateResult.NoResult();

            if (bearer[0] != "Bearer")
                return AuthenticateResult.NoResult();

            try
            {
                var tJWTUser = await authority.GetUserFromTokenAsync(bearer[1]);
                if (tJWTUser.Subject != JWTSubject.AccessToken)
                    return AuthenticateResult.Fail(new InvalidTokenException());

                return AuthenticateResult.Success(authority.CreateTicket(tJWTUser, Scheme.Name));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }));

        if (results.Any(r => r.Failure is { }))
            return results.First(r => r.Failure is { });
        if (results.Any(r => r.Succeeded))
            return results.First(r => r.Succeeded);
        return AuthenticateResult.NoResult();
    }
}