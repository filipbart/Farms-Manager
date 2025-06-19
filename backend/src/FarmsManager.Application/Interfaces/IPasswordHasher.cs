using FarmsManager.Application.Common;

namespace FarmsManager.Application.Interfaces;

public interface IPasswordHasher : IService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPasswordWithSalt);
}