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
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(HashSize);

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

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var computedHash = pbkdf2.GetBytes(HashSize);

        return CryptographicOperations.FixedTimeEquals(computedHash, originalHash);
    }
}