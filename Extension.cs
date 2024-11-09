using System.Security.Claims;
using MParchin.Authority.Model;
using MParchin.Authority.Schema;

namespace MParchin.Authority;

internal static class Extension
{
    public static long ToEpoch(this DateTime dateTime) =>
                Convert.ToInt64((dateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);

    public static DateTime ToDateTime(this long epoch) =>
        DateTimeOffset.FromUnixTimeMilliseconds(epoch).DateTime.ToUniversalTime();

    public static JWTUser ToJwtUser(this User user, string issuer, DateTime expiration, params string[] audiance) =>
            new(user)
            {
                Issuer = issuer,
                Expiration = expiration,
                Audience = audiance
            };

    public static Dictionary<string, string> ToClaimDictionary(this ClaimsPrincipal principal) =>
        principal.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
}
