using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using MParchin.Authority.Cryptography;
using MParchin.Authority.Exceptions;
using MParchin.Authority.Model;
using MParchin.Authority.Schema;

namespace MParchin.Authority.TokenFactory;

public class JWTFactory<TJWToken, TJWTUser, TUser>([FromKeyedServices(RSAKeyEnum.Private)] IRSAProvider @private,
    IJWTFactoryOption option, IAuthority<TJWTUser, TUser> authority)
    : IJWTFactory<TJWToken, TJWTUser, TUser>
    where TJWTUser : JWTUser<TUser>, new()
    where TJWToken : JWToken, new()
    where TUser : User, new()
{
    public async Task<TJWToken> RefreshAsync(string refreshToken)
    {
        var user = await authority.GetUserFromTokenAsync(refreshToken);
        if (user.Subject != JWTSubject.RefreshToken)
            throw new InvalidRefreshTokenException();
        return await SignAsync(user.User);
    }

    public virtual async Task<TJWToken> SignAsync(TUser user, TimeSpan? notBefore = null) =>
        new()
        {
            Token = await GetAccessTokenAsync(user, notBefore ?? TimeSpan.Zero),
            Expiration = DateTime.UtcNow + option.Expiration,
            RefreshToken = await GetRefreshTokenAsync(user, (notBefore ?? TimeSpan.Zero) + option.Expiration),
            RefreshExpiration = DateTime.UtcNow + option.RefresExpiration
        };

    protected async Task<string> GetTokenAsync(TJWTUser jWTUser)
    {
        var payload = JsonSerializer.Serialize(jWTUser);
        var data = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(authority.Header)) + "." +
            WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(payload));
        var sign = await Task.Run(() =>
            WebEncoders.Base64UrlEncode(@private.Provider.SignData(Encoding.UTF8.GetBytes(data),
                authority.HashAlgorithmName, authority.RSASignaturePadding)));
        return data + "." + sign;
    }

    protected virtual TJWTUser FillJWTUser(TUser user, TimeSpan notBefore)
    {
        var jwtUser = new TJWTUser();
        jwtUser.FromUser(user);
        jwtUser.Issuer = option.Authority;
        jwtUser.NotBefore = DateTime.UtcNow + notBefore;
        return jwtUser;
    }

    private Task<string> GetAccessTokenAsync(TUser user, TimeSpan notBefore)
    {
        var jwtUser = FillJWTUser(user, notBefore);
        jwtUser.Subject = JWTSubject.AccessToken;
        jwtUser.Expiration = DateTime.UtcNow + option.Expiration;
        return GetTokenAsync(jwtUser);
    }

    private Task<string> GetRefreshTokenAsync(TUser user, TimeSpan notBefore)
    {
        var jwtUser = FillJWTUser(user, notBefore);
        jwtUser.Subject = JWTSubject.RefreshToken;
        jwtUser.Expiration = DateTime.UtcNow + option.RefresExpiration;
        return GetTokenAsync(jwtUser);
    }
}