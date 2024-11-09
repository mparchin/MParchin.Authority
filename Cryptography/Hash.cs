using System.Security.Cryptography;
using MParchin.Authority.Model;

namespace MParchin.Authority.Cryptography;

public class Hash(IHashOptions options) : IHash
{
    private HashAlgorithmName HashAlgorithmName { get; } = HashAlgorithmName.SHA512;

    public void GeneratePasswordResetToken(DbUser user)
    {
        user.ResetToken = Convert.ToHexString(
            Rfc2898DeriveBytes.Pbkdf2(
                DateTime.UtcNow.ToString("U"),
                Convert.FromHexString(user.Salt),
                options.Iterations,
                HashAlgorithmName,
                options.KeySize));
        user.ResetExpirationTime = DateTime.UtcNow.Add(options.ResetTokenLifeTimeSpan);
    }

    public void SetPassword(DbUser user, string password)
    {
        user.Salt = Convert.ToHexString(RandomNumberGenerator.GetBytes(options.KeySize));
        user.Password = Convert.ToHexString(
            Rfc2898DeriveBytes.Pbkdf2(
                password,
                Convert.FromHexString(user.Salt),
                options.Iterations,
                HashAlgorithmName,
                options.KeySize));
    }

    public bool VerifyPassword(DbUser user, string password) =>
        CryptographicOperations.FixedTimeEquals(
            Rfc2898DeriveBytes.Pbkdf2(
                password,
                Convert.FromHexString(user.Salt),
                options.Iterations,
                HashAlgorithmName,
                options.KeySize), Convert.FromHexString(user.Password));
}