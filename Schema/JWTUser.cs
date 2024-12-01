using System.Security.Claims;
using System.Text.Json.Serialization;
using MParchin.Authority.JsonConverter;
using MParchin.Authority.Model;

namespace MParchin.Authority.Schema;

public class JWTUser<TUser>
    where TUser : User, new()
{
    public TUser User { get; set; } = new();
    [JsonPropertyName("iss")]
    public string Issuer { get; set; } = "";
    [JsonPropertyName("exp")]
    public DateTime Expiration { get; set; }
    [JsonPropertyName("iat")]
    [JsonConverter(typeof(DateTimeToEpochConverter))]
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("nbf")]
    [JsonConverter(typeof(DateTimeToEpochConverter))]
    public DateTime NotBefore { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("sub")]
    public string Subject { get; set; } = JWTSubject.AccessToken;

    [JsonIgnore]
    public const string NameClaimType = "name";
    [JsonIgnore]
    public const string RoleClaimType = "role";

    public JWTUser()
    {

    }

    public JWTUser(TUser user) : this()
    {
        FromUser(user);
    }

    public JWTUser(ClaimsPrincipal principal) : this()
    {
        FromClaims(principal);
    }

    public virtual void FromUser(TUser user)
    {
        User.Guid = user.Guid;
        User.Name = user.Name;
        User.Email = user.Email;
        User.Phone = user.Phone;
        User.Role = user.Role;
        User.LastLogIn = user.LastLogIn;
        User.UpdatedAt = user.UpdatedAt;
        User.CreatedAt = user.CreatedAt;
    }

    public virtual IEnumerable<Claim> GetClaims() =>
        [
            new("iss", Issuer, null, Issuer),
            new("exp", Expiration.ToEpoch().ToString(), "epoch", Issuer),
            new("iat", IssuedAt.ToEpoch().ToString(), "epoch", Issuer),
            new("nbf", NotBefore.ToEpoch().ToString(), "epoch", Issuer),
            new("sub", Subject, null, Issuer),
            new("guid", User.Guid.ToString(), "guid", Issuer),
            new(NameClaimType, User.Name, null, Issuer),
            new("email", User.Email, null, Issuer),
            new("phone", User.Phone, null, Issuer),
            new(RoleClaimType, User.Role ?? "", null, Issuer),
            new("llat", (User.LastLogIn?.ToEpoch() ?? 0).ToString(), "epoch", Issuer),
            new("uat", User.UpdatedAt.ToEpoch().ToString(), "epoch", Issuer),
            new("cat", User.CreatedAt.ToEpoch().ToString(), "epoch", Issuer),
        ];

    public virtual void FromClaims(ClaimsPrincipal principal) =>
        principal.Claims.ToList().ForEach(claim =>
        {
            switch (claim.Type)
            {
                case "iss":
                    Issuer = claim.Value;
                    break;
                case "exp":
                    Expiration = Convert.ToInt64(claim.Value).ToDateTime();
                    break;
                case "iat":
                    IssuedAt = Convert.ToInt64(claim.Value).ToDateTime();
                    break;
                case "nbf":
                    NotBefore = Convert.ToInt64(claim.Value).ToDateTime();
                    break;
                case "sub":
                    Subject = claim.Value;
                    break;
                case "guid":
                    User.Guid = Guid.Parse(claim.Value);
                    break;
                case NameClaimType:
                    User.Name = claim.Value;
                    break;
                case "email":
                    User.Email = claim.Value;
                    break;
                case "phone":
                    User.Phone = claim.Value;
                    break;
                case RoleClaimType:
                    User.Role = claim.Value;
                    break;
                case "llat":
                    User.LastLogIn = Convert.ToInt64(claim.Value).ToDateTime();
                    break;
                case "uat":
                    User.UpdatedAt = Convert.ToInt64(claim.Value).ToDateTime();
                    break;
                case "cat":
                    User.CreatedAt = Convert.ToInt64(claim.Value).ToDateTime();
                    break;
                default:
                    break;
            }
        });
}