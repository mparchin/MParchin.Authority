using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using MParchin.Authority.Cryptography;
using MParchin.Authority.Exceptions;
using MParchin.Authority.Model;
using MParchin.Authority.Schema;

namespace MParchin.Authority;

public class JWTAuthority<TJWTUser, TUser>([FromKeyedServices(RSAKeyEnum.Public)] IRSAProvider @public,
    IAuthorityOption option) : IAuthority<TJWTUser, TUser>
    where TJWTUser : JWTUser<TUser>, new()
    where TUser : User, new()
{
    private string? _header;
    public string Header => _header ??= JsonSerializer.Serialize(new
    {
        typ = "JWT",
        alg = "RS256"
    });

    public HashAlgorithmName HashAlgorithmName { get; } = HashAlgorithmName.SHA256;
    public RSASignaturePadding RSASignaturePadding { get; } = RSASignaturePadding.Pkcs1;

    public AuthenticationTicket CreateTicket(TJWTUser user, string schemaName) =>
        new(new ClaimsPrincipal(
                new ClaimsIdentity(
                    user.GetClaims(),
                    schemaName,
                    JWTUser<TUser>.NameClaimType,
                    JWTUser<TUser>.RoleClaimType)),
                new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = user.Expiration,
                    IssuedUtc = user.IssuedAt
                },
                schemaName);

    public async Task<TJWTUser> GetUserFromTokenAsync(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            throw new InvalidTokenException();
        if (parts[0] != WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Header)))
            throw new InvalidTokenException();
        if (!await VerifyTokenSignatureAsync(parts[0], parts[1], parts[2]))
            throw new InvalidTokenException();
        var user = JsonSerializer.Deserialize<TJWTUser>(
            Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(parts[1]))) ?? throw new InvalidTokenException();
        if (user.User is null)
            throw new InvalidTokenException();
        VerifyJWTUserIsValid(user);
        return user;
    }

    protected void VerifyJWTUserIsValid(TJWTUser user)
    {
        if (!option.RespectedAuthorities.Contains(user.Issuer))
            throw new AuthorityNotRespectedException();
        if (user.IssuedAt >= DateTime.UtcNow)
            throw new InvalidTokenException();
        if (user.Expiration < DateTime.UtcNow)
            throw new ExpiredTokenException();
        if (user.NotBefore > DateTime.UtcNow)
            throw new TokenIsNotYetValidException();
    }

    private Task<bool> VerifyTokenSignatureAsync(string encodedHeader, string encodedPayload, string encodedSign) =>
        Task.Run(() =>
            @public.Provider.VerifyData(Encoding.UTF8.GetBytes(encodedHeader + "." + encodedPayload),
                WebEncoders.Base64UrlDecode(encodedSign), HashAlgorithmName, RSASignaturePadding));

}