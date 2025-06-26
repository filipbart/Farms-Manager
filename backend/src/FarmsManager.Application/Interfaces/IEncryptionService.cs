using FarmsManager.Application.Common;

namespace FarmsManager.Application.Interfaces;

public interface IEncryptionService : IService
{
    string Encrypt(string plainText);
    string Decrypt(string encrypted);
}