using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.DependencyInjection;
using MParchin.Authority.Cryptography;
using MParchin.Authority.Exceptions;
using MParchin.Authority.Model;
using MParchin.Authority.Schema;
using Newtonsoft.Json;

namespace MParchin.Authority.TokenFactory;

public class JWTFactory([FromKeyedServices(KeyEnum.Private)] IRSAProvider @private,
    [FromKeyedServices(KeyEnum.Public)] IRSAProvider @public, IAuthorityToken authorityToken,
    IJWTFactoryOptions options) : IJWTFactory
{
    public JWToken Refresh(JWToken token)
    {
        var user = authorityToken.GetUser(JsonConvert
            .DeserializeObject<Dictionary<string, string>>(JwtBuilder.Create()
                .WithAlgorithm(new RS256Algorithm(@public.Key, @private.Key))
                .WithValidationParameters(JWT.ValidationParameters.Default)
                .MustVerifySignature()
                .Decode(token.RefreshToken)) ?? throw new InvalidRefreshTokenException());
        return Sign(user!);
    }

    public JWToken Sign(User user) =>
        new()
        {
            Token = JwtBuilder.Create()
                .WithAlgorithm(new RS256Algorithm(@public.Key, @private.Key))
                .AddClaims(authorityToken.GetClaims(user
                    .ToJwtUser(options.Authority, DateTime.UtcNow.Add(options.Expiration), options.Audience))
                    .Where(key => !string.IsNullOrEmpty(key.Key))
                    .Select(key => KeyValuePair.Create<string, object>(key.Key, key.Value)))
                .Encode(),
            Expiration = DateTime.UtcNow.Add(options.Expiration).ToEpoch(),
            RefreshToken = JwtBuilder.Create()
                .WithAlgorithm(new RS256Algorithm(@public.Key, @private.Key))
                .AddClaims(authorityToken.GetClaims(user
                    .ToJwtUser(options.Authority, DateTime.UtcNow.Add(options.RefresExpiration), options.Authority))
                    .Where(key => !string.IsNullOrEmpty(key.Key))
                    .Select(key => KeyValuePair.Create<string, object>(key.Key, key.Value)))
                .Encode(),
            RefreshExpiration = DateTime.UtcNow.Add(options.RefresExpiration).ToEpoch()
        };
}