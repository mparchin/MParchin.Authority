namespace MParchin.Authority.Cryptography;

public interface IHashOptions
{
    public int KeySize { get; }
    public int Iterations { get; }
}