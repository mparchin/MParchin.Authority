using System.Text.Json.Serialization;
using MParchin.Authority.JsonConverter;

namespace MParchin.Authority.Schema;

public class JWToken
{
    public string Token { get; set; } = "";
    [JsonConverter(typeof(DateTimeToEpochConverter))]
    public DateTime Expiration { get; set; }
    public string RefreshToken { get; set; } = "";
    [JsonConverter(typeof(DateTimeToEpochConverter))]
    public DateTime RefreshExpiration { get; set; }
}