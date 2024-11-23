namespace MParchin.Authority.OTP;

public interface IOTPOptions
{
    public TimeSpan Expiration { get; }
    public int OTPLength { get; }
    public string? RedisHost { get; }
    public int RedisPort { get; }
    public string? RedisUser { get; }
    public string? RedisPassword { get; }
    public int RedisDatabaseNumber { get; }
}