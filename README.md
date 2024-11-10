A micro service ready JWT Authentication and Authorization(claim based) solution using https://github.com/jwt-dotnet/jwt
you can override or extend most of the functionality to suit your needs

on authority service that is responsible for authentication of users and generating tokens the app needs a database
connection using entityframework(db should inherit from DbContext,IAuthorityDb):

    var builder = WebApplication.CreateBuilder(args);
    builder.AddJWTAuthentication<Db>(env.PUBLIC_KEY_FILE, env.PRIVATE_KEY_FILE, options =>
    {
        options.CurrentApp = env.APP;
        options.Authority = env.AUTHORITY;
        options.Audience = env.AUDIENCE.Split(",");
        options.Expiration = TimeSpan.FromHours(env.TOKEN_EXPIRATION_HOURS);
        options.RefresExpiration = TimeSpan.FromDays(env.REFRESH_EXPIRATION_DAYS);
        options.ResetTokenLifeTimeSpan = TimeSpan.FromHours(env.RESET_TOKEN_EXPIRATION_HOURS);
    });

on any other service that needs to use the token for authorization or simple identification of user:

    var builder = WebApplication.CreateBuilder(args);
    builder.AddJWTAuthentication<Db>(env.PUBLIC_KEY_FILE, options =>
    {
        options.CurrentApp = env.APP;
        options.Authority = env.AUTHORITY;
    });

and to get the user in api methods:

    app.MapGet("/", (AuthorityClaims claims) => $"Hello {claims.User.Name}!").RequireAuthorization(Authorization.User);
