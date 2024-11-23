using Microsoft.AspNetCore.Builder;

namespace MParchin.Authority.Endpoints;

public static class PublicExtension
{
    public static void MapAuthorityEndpoint(this WebApplication app, string prefix = "/") =>
        app.MapGroup(prefix)
            .MapAuthorityGroup();
}