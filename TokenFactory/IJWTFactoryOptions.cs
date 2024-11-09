namespace MParchin.Authority.TokenFactory;

public interface IJWTFactoryOptions
{
    public TimeSpan Expiration { get; }
    public TimeSpan RefresExpiration { get; }
    public string Authority { get; }
    public string[] Audience { get; }
    public string CurrentApp { get; }
}