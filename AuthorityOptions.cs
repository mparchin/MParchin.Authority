using MParchin.Authority.Cryptography;
using MParchin.Authority.TokenFactory;

namespace MParchin.Authority;

public class AuthorityOptions : IHashOptions, IJWTFactoryOptions
{
    public int KeySize { get; set; } = 128;
    public int Iterations { get; set; } = 400000;
    public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan RefresExpiration { get; set; } = TimeSpan.FromDays(7);
    public string Authority { get; set; } = "";
    public string[] Audience { get; set; } = [""];
    public string CurrentApp { get; set; } = "";
}