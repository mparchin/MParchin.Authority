using MParchin.Authority.Cryptography;
using MParchin.Authority.TokenFactory;

namespace MParchin.Authority;

public class AuthorityOptions : IHashOptions, IJWTFactoryOptions
{
    public TimeSpan ResetTokenLifeTimeSpan { get; } = TimeSpan.FromDays(1);
    public int KeySize { get; } = 128;
    public int Iterations { get; } = 400000;
    public TimeSpan Expiration { get; } = TimeSpan.FromHours(1);
    public TimeSpan RefresExpiration { get; } = TimeSpan.FromDays(7);
    public string Authority { get; } = "";
    public string[] Audience { get; } = [""];
    public string CurrentApp { get; } = "";
}