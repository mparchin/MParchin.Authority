namespace MParchin.Authority.OTP;

public class OTPOptions : IOTPOptions
{
    public TimeSpan Expiration { get; set; } = TimeSpan.FromSeconds(90);

    public string? RedisHost { get; set; }

    public int RedisPort { get; set; } = 6379;

    public string? RedisUser { get; set; }

    public string? RedisPassword { get; set; }

    public int RedisDatabaseNumber { get; set; } = 0;

    public int OTPLength { get; set; } = 6;
}