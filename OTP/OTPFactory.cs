namespace MParchin.Authority.OTP;

public class OTPFactory : IOTPFactory
{
    private readonly Random _random = new();
    public string Create() =>
        Enumerable.Range(0, 6)
            .Select(i => _random.Next(10))
            .Aggregate("", (current, next) => next + current);
}