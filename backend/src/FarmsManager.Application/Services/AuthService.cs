using System.Security.Cryptography;
using FarmsManager.Application.Interfaces;

namespace FarmsManager.Application.Services;

public class AuthService : IPasswordHasher
{
    private const int SaltSize = 16; // 128-bit
    private const int HashSize = 32; // 256-bit
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        var result = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);

        return Convert.ToBase64String(result);
    }

    public bool VerifyPassword(string password, string hashedPasswordWithSalt)
    {
        var decoded = Convert.FromBase64String(hashedPasswordWithSalt);
        if (decoded.Length != SaltSize + HashSize) return false;

        var salt = new byte[SaltSize];
        var originalHash = new byte[HashSize];
        Buffer.BlockCopy(decoded, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(decoded, SaltSize, originalHash, 0, HashSize);

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        return CryptographicOperations.FixedTimeEquals(computedHash, originalHash);
    }
}