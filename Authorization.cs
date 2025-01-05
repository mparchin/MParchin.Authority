using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace MParchin.Authority;

public static partial class Authorization
{
    private const string UserPolicyName = "User";
    public static AuthorizationPolicy User { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    private const string AdminPolicyName = "Admin";
    public static AuthorizationPolicy Admin { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("role", "admin")
        .Build();

    public static IServiceCollection AddDefaultAuthorization(this IServiceCollection services) =>
        services.AddAuthorization(options => options.AddDefaultPolicies());

    public static void AddDefaultPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(UserPolicyName, User);
        options.AddPolicy(AdminPolicyName, Admin);
    }
}