namespace MParchin.Authority.OTP;

public class OTPFactory(IOTPOptions options) : IOTPFactory
{
    private readonly Random _random = new();
    public string Create() =>
        Enumerable.Range(0, options.OTPLength)
            .Select(i => _random.Next(10))
            .Aggregate("", (current, next) => next + current);
}