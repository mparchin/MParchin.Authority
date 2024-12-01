using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MParchin.Authority.Cryptography;
using MParchin.Authority.Model;
using MParchin.Authority.OTP;
using MParchin.Authority.Schema;
using MParchin.Authority.Service;
using MParchin.Authority.TokenFactory;

namespace MParchin.Authority;

public static class JWTExtension
{
    private static IServiceCollection TryAddPublicKey(this IServiceCollection services, string publicKeyPath)
    {
        services.TryAddKeyedSingleton<IRSAProvider>(RSAKeyEnum.Public, new RSAProvider(publicKeyPath));
        return services;
    }

    private static IServiceCollection TryAddPrivateKey(this IServiceCollection services, string privateKeyPath)
    {
        services.TryAddKeyedSingleton<IRSAProvider>(RSAKeyEnum.Private, new RSAProvider(privateKeyPath));
        return services;
    }

    private static IServiceCollection TryAddOptions(this IServiceCollection services, Action<AuthorityOptions>? optionFunc = null)
    {
        if (services.Any(sd => sd.ServiceType == typeof(IJWTFactoryOption)))
            return services;
        var options = new AuthorityOptions();
        optionFunc?.Invoke(options);
        services.TryAddSingleton<IJWTFactoryOption>(options);
        services.TryAddSingleton<IHashOptions>(options);
        services.TryAddSingleton<IAuthorityOption>(options);
        return services;
    }

    public static IServiceCollection AddMailService<TMail>(this IServiceCollection services)
        where TMail : class, IMail =>
        services.AddSingleton<IMail, TMail>();

    public static IServiceCollection AddTextMessageService<TTextMessage>(IServiceCollection services)
        where TTextMessage : class, ITextMessage =>
        services.AddSingleton<ITextMessage, TTextMessage>();

    public static IServiceCollection AddOTPStorageService(this IServiceCollection services,
        Action<OTPOptions>? configuration = null)
    {
        var options = new OTPOptions();
        configuration?.Invoke(options);
        services.AddSingleton<IOTPOptions>(options);
        return string.IsNullOrEmpty(options.RedisHost)
            ? services.AddSingleton<IStorage, MemoryStorage>()
            : services.AddSingleton<IStorage, RedisStorage>();
    }

    private static void AddJwt<TJWTUser, TUser>(this AuthenticationBuilder builder)
        where TJWTUser : JWTUser<TUser>, new()
        where TUser : User, new() =>
        builder.AddScheme<JWTAuthenticationOptions, JWTAuthenticationHandler<TJWTUser, TUser>>(
            JWTAuthenticationDefaults.AuthenticationScheme, null);

    public static IServiceCollection AddJWTAuthentication<TJWTUser, TUser>(this IServiceCollection services,
        string publicKeyPath, Action<AuthorityOptions>? optionFunc)
        where TJWTUser : JWTUser<TUser>, new()
        where TUser : User, new()
    {
        services.TryAddPublicKey(publicKeyPath)
            .TryAddOptions(optionFunc)
            .AddSingleton<IAuthority<TJWTUser, TUser>, JWTAuthority<TJWTUser, TUser>>()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JWTAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JWTAuthenticationDefaults.AuthenticationScheme;
            }).AddJwt<TJWTUser, TUser>();
        return services;
    }

    public static IServiceCollection AddJWTAuthentication<TJWToken, TJWTUser, TDbUser, TUser, TDb>(
        this IServiceCollection services, string publicKeyPath, string privateKeyPath, Action<AuthorityOptions>? optionFunc = null)
        where TDb : class, IAuthorityDb<TDbUser>
        where TDbUser : User, TUser, IDbUser, new()
        where TJWTUser : JWTUser<TUser>, new()
        where TJWToken : JWToken, new()
        where TUser : User, new()
    {
        services.TryAddSingleton<ITextMessage, DefaultTextMessage>();
        services.TryAddSingleton<IOTPOptions, OTPOptions>();
        services.TryAddSingleton<IStorage, MemoryStorage>();
        services.TryAddSingleton<IMail, DefaultMail>();
        services.TryAddPrivateKey(privateKeyPath)
            .TryAddPublicKey(publicKeyPath)
            .TryAddOptions(optionFunc)
            .AddScoped<IAuthorityDb<TDbUser>>(sp => sp.GetService<TDb>()!)
            .AddSingleton<IJWTFactory<TJWToken, TJWTUser, TUser>, JWTFactory<TJWToken, TJWTUser, TUser>>()
            .AddSingleton<IOTPFactory, OTPFactory>()
            .AddSingleton<IHash<TDbUser>, Hash<TDbUser>>()
            .AddScoped<IAuthorityService<TDbUser, TUser>, AuthorityService<TDbUser, TUser>>();
        return services.AddJWTAuthentication<TJWTUser, TUser>(publicKeyPath, optionFunc);
    }

    public static void UseMemoryStorage(this OTPOptions options, TimeSpan? expiration = null, int OTPLength = 6)
    {
        options.Expiration = expiration ?? TimeSpan.FromSeconds(90);
        options.RedisHost = null;
        options.OTPLength = OTPLength;
    }

    public static void UseRedisStorage(this OTPOptions options, string host, int port = 6379,
        string? user = null, string? password = null, int databaseNumber = 0, TimeSpan? expiration = null, int OTPLength = 6)
    {
        options.RedisHost = host;
        options.RedisPort = port;
        options.RedisUser = user;
        options.RedisPassword = password;
        options.RedisDatabaseNumber = databaseNumber;
        options.Expiration = expiration ?? TimeSpan.FromSeconds(90);
        options.OTPLength = OTPLength;
    }
}
