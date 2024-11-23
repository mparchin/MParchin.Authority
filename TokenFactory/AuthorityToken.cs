using System.Security.Claims;
using MParchin.Authority.Exceptions;
using MParchin.Authority.Schema;

namespace MParchin.Authority.TokenFactory
{
    public class AuthorityToken(IJWTFactoryOptions options) : IAuthority
    {
        protected virtual List<(string key, Func<JWTUser, string> get, Action<JWTUser, string> set)> Keys { get; } =
        [
            ("iss", (user) => user.Issuer, (user, value) => user.Issuer = value),
            ("sub", (user) => user.Guid.ToString(), (user, value) => user.Guid = Guid.Parse(value)),
            ("aud", (user) => user.Audience.Aggregate("", (current,next) => current == "" ? next : $"{current},{next}"),
                (user, value) => user.Audience = value.Split(",")),
            ("exp", (user) => user.Expiration.ToEpoch().ToString(),
                (user, value) => user.Expiration = Convert.ToInt64(value).ToDateTime()),
            ("iat", (user) => user.IssuedAt.ToEpoch().ToString(),
                (user, value) => user.IssuedAt = Convert.ToInt64(value).ToDateTime()),
            ("name", (user) => user.Name, (user, value) => user.Name = value),
            ("email", (user) => user.Email, (user, value) => user.Email = value),
            ("phone", (user) => user.Phone, (user, value) => user.Phone = value),
            ("role", (user) => user.Role ?? "", (user, value) => user.Role = value),
            ("llat", (user) => user.LastLogIn?.ToEpoch().ToString() ?? "",
                (user, value) => user.LastLogIn = Convert.ToInt64(value).ToDateTime()),
            ("upat", (user) => user.UpdatedAt.ToEpoch().ToString(),
                (user, value) => user.UpdatedAt = Convert.ToInt64(value).ToDateTime()),
        ];

        public void VerifyJWTToken(JWTUser user)
        {
            if (user.Issuer != options.Authority)
                throw new UnrecognizedIssuerAuthorityException();
            if (!user.Audience.Contains(options.CurrentApp))
                throw new NotInAudienceException();
            if (user.Expiration <= DateTime.UtcNow)
                throw new ExpiredTokenException();
            if (user.IssuedAt >= DateTime.UtcNow)
                throw new InvalidTokenException();
        }

        public JWTUser GetUser(Dictionary<string, string> claims)
        {
            var user = new JWTUser();
            claims.ToList().ForEach(claim =>
            {
                if (Keys.FirstOrDefault(key => key.key == claim.Key) is { } found)
                    found.set(user, claim.Value);
            });
            return user;
        }

        public Dictionary<string, string> GetClaims(JWTUser user) =>
            Keys.ToDictionary(key => key.key, key => key.get(user));

        public JWTUser GetUser(ClaimsPrincipal principal) =>
            GetUser(principal.ToClaimDictionary());
    }
}