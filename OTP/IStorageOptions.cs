namespace MParchin.Authority.OTP;

public interface IStorageOptions
{
    public TimeSpan Expiration { get; }
    public string? RedisHost { get; }
    public int RedisPort { get; }
    public string? RedisUser { get; }
    public string? RedisPassword { get; }
    public int RedisDatabaseNumber { get; }
}