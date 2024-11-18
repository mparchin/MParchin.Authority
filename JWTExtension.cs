using JWT.Algorithms;
using JWT.Extensions.AspNetCore;
using JWT.Extensions.AspNetCore.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MParchin.Authority.Cryptography;
using MParchin.Authority.OTP;
using MParchin.Authority.Service;
using MParchin.Authority.TokenFactory;

namespace MParchin.Authority;

public static class JWTExtension
{
    private static IServiceCollection TryAddPublicKey(this IServiceCollection services, string publicKeyPath)
    {
        services.TryAddKeyedSingleton<IRSAProvider>(KeyEnum.Public, new RSAProvider(publicKeyPath));
        return services;
    }

    private static IServiceCollection TryAddPrivateKey(this IServiceCollection services, string privateKeyPath)
    {
        services.TryAddKeyedSingleton<IRSAProvider>(KeyEnum.Private, new RSAProvider(privateKeyPath));
        return services;
    }

    private static IServiceCollection TryAddOptions(this IServiceCollection services, Action<AuthorityOptions>? optionFunc = null)
    {
        if (services.Any(sd => sd.ServiceType == typeof(IJWTFactoryOptions)))
            return services;
        var options = new AuthorityOptions();
        optionFunc?.Invoke(options);
        services.TryAddSingleton<IJWTFactoryOptions>(options);
        services.TryAddSingleton<IHashOptions>(options);
        return services;
    }

    public static IServiceCollection AddMailService<TMail>(this IServiceCollection services)
        where TMail : class, IMail =>
        services.AddSingleton<IMail, TMail>();

    public static IServiceCollection AddTextMessageService<TTextMessage>(IServiceCollection services)
        where TTextMessage : class, ITextMessage =>
        services.AddSingleton<ITextMessage, TTextMessage>();

    public static IServiceCollection AddOTPStorageService(this IServiceCollection services,
        Action<StorageOptions>? configuration = null)
    {
        var options = new StorageOptions();
        configuration?.Invoke(options);
        services.AddSingleton<IStorageOptions>(options);
        return string.IsNullOrEmpty(options.RedisHost)
            ? services.AddSingleton<IStorage, MemoryStorage>()
            : services.AddSingleton<IStorage, RedisStorage>();
    }

    public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, string publicKeyPath,
        Action<AuthorityOptions>? optionFunc = null) =>
        services.AddJWTAuthentication<AuthorityToken>(publicKeyPath, optionFunc);

    public static IServiceCollection AddJWTAuthentication<TAuthorityToken>(this IServiceCollection services, string publicKeyPath,
        Action<AuthorityOptions>? optionFunc)
    where TAuthorityToken : class, IAuthorityToken
    {
        services.TryAddPublicKey(publicKeyPath)
            .TryAddOptions(optionFunc)
            .AddSingleton<IAuthorityToken, TAuthorityToken>()
            .AddSingleton<IAlgorithmFactory>(sp => new RSAlgorithmFactory(sp.GetRequiredKeyedService<IRSAProvider>(KeyEnum.Public).Key))
            .AddSingleton<IIdentityFactory, TokenFactory.ClaimsIdentityFactory>()
            .AddScoped<AuthorityClaims>()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtAuthenticationDefaults.AuthenticationScheme;
            }).AddJwt();
        return services;
    }

    public static IServiceCollection AddJWTAuthentication<TDb>(this IServiceCollection services,
        string publicKeyPath, string privateKeyPath, Action<AuthorityOptions>? optionFunc = null)
        where TDb : class, IAuthorityDb =>
        services.AddJWTAuthentication<AuthorityToken, TDb, JWTFactory, Hash, AuthorityService>(publicKeyPath, privateKeyPath, optionFunc);

    public static IServiceCollection AddJWTAuthentication<TAuthorityToken, TDb, TJWTFactory, THash, TAuthorityService>(
        this IServiceCollection services, string publicKeyPath, string privateKeyPath, Action<AuthorityOptions>? optionFunc = null)
        where TAuthorityToken : class, IAuthorityToken
        where TDb : class, IAuthorityDb
        where TJWTFactory : class, IJWTFactory
        where THash : class, IHash
        where TAuthorityService : class, IAuthorityService
    {
        services.TryAddSingleton<ITextMessage, DefaultTextMessage>();
        services.TryAddSingleton<IStorageOptions, StorageOptions>();
        services.TryAddSingleton<IStorage, MemoryStorage>();
        services.TryAddSingleton<IMail, DefaultMail>();
        services.TryAddPrivateKey(privateKeyPath)
            .TryAddPublicKey(publicKeyPath)
            .TryAddOptions(optionFunc)
            .AddScoped<IAuthorityDb>(sp => sp.GetService<TDb>()!)
            .AddSingleton<IJWTFactory, TJWTFactory>()
            .AddSingleton<IHash, THash>()
            .AddScoped<IAuthorityService, TAuthorityService>();
        return services.AddJWTAuthentication<TAuthorityToken>(publicKeyPath, optionFunc);
    }

    public static void UseMemoryStorage(this StorageOptions options, TimeSpan? expiration = null)
    {
        options.Expiration = expiration ?? TimeSpan.FromSeconds(90);
        options.RedisHost = null;
    }

    public static void UseRedisStorage(this StorageOptions options, string host, int port = 6379,
        string? user = null, string? password = null, int databaseNumber = 0, TimeSpan? expiration = null)
    {
        options.RedisHost = host;
        options.RedisPort = port;
        options.RedisUser = user;
        options.RedisPassword = password;
        options.RedisDatabaseNumber = databaseNumber;
        options.Expiration = expiration ?? TimeSpan.FromSeconds(90);
    }
}