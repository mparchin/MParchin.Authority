
namespace MParchin.Authority.OTP;

public class MemoryStorage(IStorageOptions options) : IStorage
{
    private readonly Dictionary<string, (string otp, DateTime CreationTime)> _memory = [];
    public Task<bool> ConfirmAndRemoveAsync(string username, string otp)
    {
        var ret = _memory.Any(pair => pair.Key == username && pair.Value.otp == otp &&
            pair.Value.CreationTime + options.Expiration < DateTime.UtcNow);

        if (ret || _memory.Any(pair => pair.Key == username && pair.Value.otp == otp))
            _memory.Remove(username);

        return Task.FromResult(ret);
    }

    public Task StoreAsync(string username, string otp)
    {
        if (_memory.ContainsKey(username))
            _memory[username] = (otp, DateTime.UtcNow);
        else
            _memory.Add(username, (otp, DateTime.UtcNow));

        return Task.CompletedTask;
    }
}