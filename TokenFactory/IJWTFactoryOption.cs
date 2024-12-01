namespace MParchin.Authority.TokenFactory;

public interface IJWTFactoryOption
{
    public TimeSpan Expiration { get; }
    public TimeSpan RefresExpiration { get; }
    public string Authority { get; }
}