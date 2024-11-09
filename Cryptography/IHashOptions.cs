namespace MParchin.Authority.Cryptography;

public interface IHashOptions
{
    public TimeSpan ResetTokenLifeTimeSpan { get; }
    public int KeySize { get; }
    public int Iterations { get; }
}