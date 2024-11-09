using Microsoft.AspNetCore.Authorization;

namespace MParchin.Authority;

public static partial class Authorization
{
    public static AuthorizationPolicy User { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    public static AuthorizationPolicy Admin { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("role", "admin")
        .Build();
}