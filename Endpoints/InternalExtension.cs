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

namespace MParchin.Authority.Endpoints;

internal static class InternalExtension
{
    internal static void MapAuthorityGroup(this RouteGroupBuilder group)
    {
        // group.MapPost("/register", RegisterAsync);
        group.MapPost("/register", RegisterOTPAsync)
            .AllowAnonymous()
            .WithTags("Authority")
            .WithDescription("Register users using otp from \"/otp/{username}\" route");

        group.MapPost("/refresh", Refresh)
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

    internal static async Task<Results<BadRequest, Ok<JWToken>>> RegisterOTPAsync(RegisterOTPRequest request,
        IAuthorityService service, IJWTFactory factory)
    {
        try
        {
            var user = await service.SignUpOTPAsync(new User
            {
                Email = request.Username.Contains('@') ? request.Username : "",
                Phone = request.Username.Contains('@') ? "" : request.Username,
                Name = request.Name,
            }, request.Password, request.Otp);
            return TypedResults.Ok(factory.Sign(user));
        }
        catch { }
        return TypedResults.BadRequest();
    }

    internal static async Task<Ok<PublicUserInfo>> GetUserPublicInfoAsync([FromRoute] string username, IAuthorityService service)
    {
        if (!await service.ExistsAsync(username))
            return TypedResults.Ok(new PublicUserInfo
            {
                Name = null,
                Username = username,
            });

        return TypedResults.Ok(new PublicUserInfo
        {
            Name = (await service.GetAsync(username)).Name,
            Username = username,
        });
    }

    internal static Results<UnauthorizedHttpResult, Ok<JWToken>> Refresh(JWToken token, IJWTFactory factory)
    {
        try
        {
            return TypedResults.Ok(factory.Refresh(token));
        }
        catch { }
        return TypedResults.Unauthorized();
    }

    internal static async Task<Results<UnauthorizedHttpResult, Ok<JWToken>>> LoginAsync(LoginRequest request,
        IAuthorityService service, IJWTFactory factory)
    {
        try
        {
            return TypedResults.Ok(factory.Sign(await service.SignInAsync(request.Username, request.Password)));
        }
        catch { }
        return TypedResults.Unauthorized();
    }

    internal static async Task<Results<UnauthorizedHttpResult, Ok<JWToken>>> LoginOTPAsync(LoginOTPRequest request,
        IAuthorityService service, IJWTFactory factory)
    {
        try
        {
            return TypedResults.Ok(factory.Sign(await service.SignInOTPAsync(request.Username, request.Otp)));
        }
        catch { }
        return TypedResults.Unauthorized();
    }

    internal static async Task<Results<BadRequest, Ok>> GenerateOTPAsync([FromRoute] string username,
        IAuthorityService service)
    {
        try
        {
            if (username.Contains('@'))
                await service.GenerateEmailOTPAsync(username);
            else
                await service.GeneratePhoneOTPAsync(username);
            return TypedResults.Ok();
        }
        catch { }
        return TypedResults.BadRequest();
    }

    internal static async Task<Results<BadRequest, Ok>> ChangePasswordAsync([FromRoute] string username,
        ChangePasswordRequest request, IAuthorityService service)
    {
        try
        {
            await service.ChangePasswordAsync(username, request.Otp, request.NewPassword);
            return TypedResults.Ok();
        }
        catch { }
        return TypedResults.BadRequest();
    }

    internal static Ok<User> Me(IAuthority authority, ClaimsPrincipal principal) =>
        TypedResults.Ok<User>(authority.GetUser(principal));
}