using System.Security.Cryptography;
using TaskTrack.Application.Interfaces;

namespace TaskTrack.Infrastructure.Auth;

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 10_000;
    private const int KeySize = 32;

    public (string Hash, string Salt) HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA256, KeySize);
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool Verify(string password, string passwordHash, string passwordSalt)
    {
        var saltBytes = Convert.FromBase64String(passwordSalt);
        var computed = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA256, KeySize);
        return CryptographicOperations.FixedTimeEquals(computed, Convert.FromBase64String(passwordHash));
    }
}
