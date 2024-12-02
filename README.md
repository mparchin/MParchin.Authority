A micro service ready JWT Authentication and Authorization(claim based) solution you can override or
extend most of the functionality to suit your needs, almost every functionality is generic with respect to
User model so you can use your own model in place of any predefined types

on authority service that is responsible for authentication of users and generating tokens the app needs a database
connection using entityframework(db should inherit from DbContext,IAuthorityDb<TDbUser>):

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddJWTAuthentication<JWToken, JWTUser<User>, DbUser, User, Db>(
        env.PUBLIC_KEY_FILE, env.PRIVATE_KEY_FILE, options =>
        {
            options.Expiration = TimeSpan.FromHours(env.TOKEN_EXPIRATION_HOURS);
            options.RefresExpiration = TimeSpan.FromDays(env.REFRESH_EXPIRATION_DAYS);
            options.Authority = env.AUTHORITY;
        });
    ....
    AuthorityEndpoint<JWToken, JWTUser<User>, DbUser, User>.Map(app, "/"); // adding authority api methods

adding mail service:[optional]

    builder.Services.AddMailService<MailService>();

adding text-message service:[optional]

    builder.Services.AddTextMessageService<TextMessageService>();

by default authority uses in memory storage for otp storage you can also configure it to use redis database:[optional]

    builder.Services.AddOTPStorageService(options => options.UseRedisStorage(env.REDIS_HOST));

on any other service that needs to use the token for authorization or simple identification of user:

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddJWTAuthentication<JWTUser<User>, User>(env.PUBLIC_KEY_FILE, options =>
    {
        options.RespectedAuthorities = env.RESPECTED_AUTHORITIES.Split(',');
    });

and to get the user in api methods:

    app.MapGet("/", (ClaimsPrincipal principal) => $"Hello {principal.Identity?.Name}!")
        .RequireAuthorization(Authorization.User);

    ...
    or
    ...

    var user = new JWTUser(principal);

also you need a pem encoded rsa key pair for authority service and just the public one for other
services you can generate a pair using these commands:

    ssh-keygen -t rsa -b 2048 -m PEM -f key
    openssl rsa -in key -pubout -outform PEM -out key.pub
