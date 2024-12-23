using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MParchin.Authority.Model;
using MParchin.Authority.Schema;
using MParchin.Authority.Service;
using MParchin.Authority.TokenFactory;

namespace MParchin.Authority;

public static class AuthorityEndpoint<TJWToken, TJWTUser, TDbUser, TUser>
    where TDbUser : TUser, IDbUser, new()
    where TUser : User, new()
    where TJWTUser : JWTUser<TUser>, new()
    where TJWToken : JWToken
{
    public static void Map(WebApplication app, string prefix = "/") =>
        MapAuthorityGroup(app.MapGroup(prefix));

    private static void MapAuthorityGroup(RouteGroupBuilder group)
    {
        // group.MapPost("/register", RegisterAsync);
        group.MapPost("/register", RegisterOTPAsync)
            .AllowAnonymous()
            .WithTags("Authority")
            .WithDescription("Register users using otp from \"/otp/{username}\" route");

        group.MapPost("/refresh", RefreshAsync)
            .WithTags("Authority")
            .WithDescription("Refresh access token");

        group.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .WithTags("Authority")
            .WithDescription("Login users using username and password");

        group.MapPost("/otp/login", LoginOTPAsync)
            .AllowAnonymous()
            .WithTags("Authority")
            .WithDescription("Login users using username and otp");
        group.MapPost("/otp/{username}", GenerateOTPAsync)
            .AllowAnonymous()
            .WithTags("Authority")
            .WithDescription("Request otp to be send to username (either phone or email)");

        group.MapGet("/users/{username}", GetUserPublicInfoAsync)
            .AllowAnonymous()
            .WithTags("Users")
            .WithDescription("Get public information of a single user or its existance");
        group.MapPost("/users/{username}/changepassword", ChangePasswordAsync)
            .RequireAuthorization(Authorization.User)
            .WithTags("Authority")
            .WithDescription("Change password using otp from \"/otp/{username}\" route");

        group.MapGet("/users/me", Me)
            .RequireAuthorization(Authorization.User)
            .WithTags("Users")
            .WithDescription("Get current token information");
    }

    // internal static async Task<Results<BadRequest, Ok<JWToken>>> RegisterAsync(RegisterInfo info,
    //     IAuthorityService service, IJWTFactory factory)
    // {
    //     try
    //     {
    //         var user = await service.SignUpAsync(new User
    //         {
    //             Email = info.Username.Contains('@') ? info.Username : "",
    //             Phone = info.Username.Contains('@') ? "" : info.Username,
    //             Name = info.Name,
    //         }, info.Password);
    //         return TypedResults.Ok(factory.Sign(user));
    //     }
    //     catch { }
    //     return TypedResults.BadRequest();
    // }

    private static async Task<Results<BadRequest<string>, Ok<TJWToken>>> RegisterOTPAsync(
        RegisterOTPRequest request, IAuthorityService<TDbUser, TUser> service, IJWTFactory<TJWToken, TJWTUser, TUser> factory)
    {
        try
        {
            var user = await service.SignUpOTPAsync(new TUser
            {
                Email = request.Username.Contains('@') ? request.Username : "",
                Phone = request.Username.Contains('@') ? "" : request.Username,
                Name = request.Name,
            }, request.Password, request.Otp);
            return TypedResults.Ok(await factory.SignAsync(user));
        }
        catch (Exception e)
        {
            return TypedResults.BadRequest(e.Message);
        }
    }

    private static async Task<Results<NotFound, Ok<PublicUserInfo>>> GetUserPublicInfoAsync(
        [FromRoute] string username, IAuthorityService<TDbUser, TUser> service)
    {
        if (!await service.ExistsAsync(username))
            return TypedResults.NotFound();

        return TypedResults.Ok(new PublicUserInfo
        {
            Name = (await service.GetAsync(username)).Name,
            Username = username,
        });
    }

    private static async Task<Results<UnauthorizedHttpResult, Ok<TJWToken>>> RefreshAsync(
        TJWToken token, IJWTFactory<TJWToken, TJWTUser, TUser> factory)
    {
        try
        {
            return TypedResults.Ok(await factory.RefreshAsync(token.RefreshToken));
        }
        catch { }
        return TypedResults.Unauthorized();
    }

    private static async Task<Results<UnauthorizedHttpResult, Ok<TJWToken>>> LoginAsync(
        LoginRequest request, IAuthorityService<TDbUser, TUser> service, IJWTFactory<TJWToken, TJWTUser, TUser> factory)
    {
        try
        {
            return TypedResults.Ok(await factory.SignAsync(await service.SignInAsync(request.Username, request.Password)));
        }
        catch { }
        return TypedResults.Unauthorized();
    }

    private static async Task<Results<UnauthorizedHttpResult, Ok<TJWToken>>> LoginOTPAsync(
        LoginOTPRequest request, IAuthorityService<TDbUser, TUser> service, IJWTFactory<TJWToken, TJWTUser, TUser> factory)
    {
        try
        {
            return TypedResults.Ok(await factory.SignAsync(await service.SignInOTPAsync(request.Username, request.Otp)));
        }
        catch { }
        return TypedResults.Unauthorized();
    }

    private static async Task<Results<BadRequest<string>, Ok>> GenerateOTPAsync([FromRoute] string username,
        IAuthorityService<TDbUser, TUser> service)
    {
        try
        {
            if (username.Contains('@'))
                await service.GenerateEmailOTPAsync(username);
            else
                await service.GeneratePhoneOTPAsync(username);
            return TypedResults.Ok();
        }
        catch (Exception e)
        {
            return TypedResults.BadRequest(e.Message);
        }
    }

    private static async Task<Results<BadRequest<string>, Ok>> ChangePasswordAsync([FromRoute] string username,
        ChangePasswordRequest request, IAuthorityService<TDbUser, TUser> service)
    {
        try
        {
            await service.ChangePasswordAsync(username, request.Otp, request.NewPassword);
            return TypedResults.Ok();
        }
        catch (Exception e)
        {
            return TypedResults.BadRequest(e.Message);
        }
    }

    private static Ok<TUser> Me(ClaimsPrincipal principal)
    {
        var jwtUser = new TJWTUser();
        jwtUser.FromClaims(principal);
        return TypedResults.Ok(jwtUser.User);
    }
}