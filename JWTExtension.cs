using JWT.Algorithms;
using JWT.Extensions.AspNetCore;
using JWT.Extensions.AspNetCore.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MParchin.Authority.Cryptography;
using MParchin.Authority.Service;
using MParchin.Authority.TokenFactory;

namespace MParchin.Authority;

public static class JWTExtension
{
    private static void TryAddPublicKey(this WebApplicationBuilder builder, string publicKeyPath) =>
        builder.Services.TryAddKeyedSingleton<IRSAProvider>(KeyEnum.Public, new RSAProvider(publicKeyPath));

    private static void TryAddPrivateKey(this WebApplicationBuilder builder, string privateKeyPath) =>
        builder.Services.TryAddKeyedSingleton<IRSAProvider>(KeyEnum.Private, new RSAProvider(privateKeyPath));

    private static void TryAddOptions(this WebApplicationBuilder builder, Action<IJWTFactoryOptions>? optionFunc = null)
    {
        if (builder.Services.Any(sd => sd.ServiceType == typeof(IJWTFactoryOptions)))
            return;
        var options = new AuthorityOptions();
        optionFunc?.Invoke(options);
        builder.Services.TryAddSingleton<IJWTFactoryOptions>(options);
        builder.Services.TryAddSingleton<IHashOptions>(options);
    }

    public static void AddJWTAuthentication(this WebApplicationBuilder builder, string publicKeyPath,
        Action<IJWTFactoryOptions>? optionFunc = null) =>
        builder.AddJWTAuthentication<AuthorityToken>(publicKeyPath, optionFunc);

    public static void AddJWTAuthentication<TAuthorityToken>(this WebApplicationBuilder builder, string publicKeyPath,
        Action<IJWTFactoryOptions>? optionFunc)
    where TAuthorityToken : class, IAuthorityToken
    {
        builder.TryAddPublicKey(publicKeyPath);
        builder.TryAddOptions(optionFunc);
        builder.Services.AddSingleton<IAuthorityToken, TAuthorityToken>()
            .AddSingleton<IAlgorithmFactory>(sp => new RSAlgorithmFactory(sp.GetRequiredKeyedService<IRSAProvider>(KeyEnum.Public).Key))
            .AddSingleton<IIdentityFactory, TokenFactory.ClaimsIdentityFactory>()
            .AddScoped<AuthorityClaims>()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtAuthenticationDefaults.AuthenticationScheme;
            }).AddJwt();
    }

    public static void AddJWTAuthentication<TDb>(this WebApplicationBuilder builder,
        string publicKeyPath, string privateKeyPath, Action<IJWTFactoryOptions>? optionFunc = null)
        where TDb : class, IAuthorityDb =>
        builder.AddJWTAuthentication<AuthorityToken, TDb, JWTFactory, Hash, AuthorityService>(publicKeyPath, privateKeyPath, optionFunc);

    public static void AddJWTAuthentication<TAuthorityToken, TDb, TJWTFactory, THash, TAuthorityService>(this WebApplicationBuilder builder,
        string publicKeyPath, string privateKeyPath, Action<IJWTFactoryOptions>? optionFunc = null)
        where TAuthorityToken : class, IAuthorityToken
        where TDb : class, IAuthorityDb
        where TJWTFactory : class, IJWTFactory
        where THash : class, IHash
        where TAuthorityService : class, IAuthorityService
    {
        builder.TryAddPrivateKey(privateKeyPath);
        builder.TryAddPublicKey(publicKeyPath);
        builder.TryAddOptions(optionFunc);
        builder.Services.AddScoped<IAuthorityDb>(sp => sp.GetService<TDb>()!)
            .AddSingleton<IJWTFactory, TJWTFactory>()
            .AddSingleton<IHash, THash>()
            .AddScoped<IAuthorityService, TAuthorityService>();
        builder.AddJWTAuthentication<TAuthorityToken>(publicKeyPath, optionFunc);
    }
}