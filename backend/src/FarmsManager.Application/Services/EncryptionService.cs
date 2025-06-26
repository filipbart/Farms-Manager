using System.Security.Cryptography;
using System.Text;
using FarmsManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FarmsManager.Application.Services;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(IConfiguration configuration)
    {
        var hexKey = configuration.GetValue<string>("Encryption:Key");
        _key = Convert.FromHexString(hexKey);
    }


    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var iv = Convert.ToHexString(aes.IV);
        var cipher = Convert.ToHexString(cipherBytes);
        return $"{iv}:{cipher}";
    }

    public string Decrypt(string encrypted)
    {
        var parts = encrypted.Split(':');
        var iv = Convert.FromHexString(parts[0]);
        var cipherBytes = Convert.FromHexString(parts[1]);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}