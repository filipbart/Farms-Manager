using Ardalis.GuardClauses;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.UserAggregate.Entities;

public class UserEntity : Entity
{
    protected UserEntity()
    {
    }

    public string Login { get; init; }
    public string PasswordHash { get; protected internal set; }
    public string Name { get; protected internal set; }

    public static UserEntity CreateUser(string login, string name)
    {
        var user = new UserEntity
        {
            Login = login
        };

        user.ChangeName(name);
        return user;
    }

    public void ChangePassword(string passwordHash)
    {
        Guard.Against.NullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
    }

    public void ChangeName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Name = name;
    }
}