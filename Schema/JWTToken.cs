namespace MParchin.Authority.Schema;

public class JWToken
{
    public string? Token { get; set; } = "";
    public long? Expiration { get; set; }
    public string? RefreshToken { get; set; } = "";
    public long? RefreshExpiration { get; set; }
}