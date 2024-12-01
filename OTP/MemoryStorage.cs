namespace MParchin.Authority.OTP;

public class MemoryStorage(IOTPOptions options) : IStorage
{
    private readonly Dictionary<string, (string otp, DateTime CreationTime)> _memory = [];
    public Task<bool> ConfirmAndRemoveAsync(string username, string otp)
    {
        var ret = _memory.Any(pair => pair.Key == username && pair.Value.otp == otp &&
            pair.Value.CreationTime + options.Expiration > DateTime.UtcNow);

        if (ret)
            _memory.Remove(username);

        RemoveExpired();

        return Task.FromResult(ret);
    }

    public Task<bool> ExistsAsync(string username)
    {
        RemoveExpired();
        return Task.FromResult(_memory.ContainsKey(username));
    }

    public Task StoreAsync(string username, string otp)
    {
        if (_memory.ContainsKey(username))
            _memory[username] = (otp, DateTime.UtcNow);
        else
            _memory.Add(username, (otp, DateTime.UtcNow));

        RemoveExpired();

        return Task.CompletedTask;
    }

    private void RemoveExpired() =>
        _memory.Where((pair) => pair.Value.CreationTime + options.Expiration <= DateTime.UtcNow)
            .Select(pair => pair.Key)
            .ToList()
            .ForEach(key => _memory.Remove(key));
}