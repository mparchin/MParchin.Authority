A micro service ready JWT Authentication and Authorization(claim based) solution using https://github.com/jwt-dotnet/jwt
you can override or extend most of the functionality to suit your needs

on authority service that is responsible for authentication of users and generating tokens the app needs a database
connection using entityframework(db should inherit from DbContext,IAuthorityDb):

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddJWTAuthentication<Db>(env.PUBLIC_KEY_FILE, env.PRIVATE_KEY_FILE, options =>
    {
        options.CurrentApp = env.APP;
        options.Authority = env.AUTHORITY;
        options.Audience = env.AUDIENCE.Split(",");
        options.Expiration = TimeSpan.FromHours(env.TOKEN_EXPIRATION_HOURS);
        options.RefresExpiration = TimeSpan.FromDays(env.REFRESH_EXPIRATION_DAYS);
    });
    ....
    app.MapAuthorityEndpoint("/"); // adding authority api methods

adding mail service:[optional]

    builder.Services.AddMailService<MailService>();

adding text-message service:[optional]

    builder.Services.AddTextMessageService<TextMessageService>();

by default authority uses in memory storage for otp storage you can also configure it to use redis database:[optional]

    builder.Services.AddOTPStorageService(options => options.UseRedisStorage(env.REDIS_HOST));

on any other service that needs to use the token for authorization or simple identification of user:

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddJWTAuthentication<Db>(env.PUBLIC_KEY_FILE, options =>
    {
        options.CurrentApp = env.APP;
        options.Authority = env.AUTHORITY;
    });

and to get the user in api methods:

    app.MapGet("/", (ClaimsPrincipal principal, IAuthority authority) => $"Hello {authority.GetUser(principal)}!")
        .RequireAuthorization(Authorization.User);
