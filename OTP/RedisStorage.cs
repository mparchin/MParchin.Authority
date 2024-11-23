using StackExchange.Redis;

namespace MParchin.Authority.OTP;

public class RedisStorage(IOTPOptions options) : IStorage
{
    private readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
    {
        EndPoints =
        {
            { options.RedisHost!, options.RedisPort }
        },
        User = options.RedisUser,
        Password = options.RedisPassword,
        DefaultDatabase = options.RedisDatabaseNumber,
    });

    public async Task<bool> ConfirmAndRemoveAsync(string username, string otp)
    {
        var database = _redis.GetDatabase();
        var read = await database.StringGetAsync(username);
        if (read == otp)
        {
            await database.KeyDeleteAsync(username);
            return true;
        }
        return false;
    }

    public async Task<bool> ExistsAsync(string username)
    {
        var database = _redis.GetDatabase();
        return await database.KeyExistsAsync(username);
    }

    public async Task StoreAsync(string username, string otp)
    {
        var database = _redis.GetDatabase();
        await database.StringSetAsync(username, otp, options.Expiration);
    }
}