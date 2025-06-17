using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.UserAggregate.Entites;

public class UserEntity : Entity
{
    protected UserEntity()
    {
    }

    public string Login { get; init; }
    public string PasswordHash { get; protected internal set; }
    public string Name { get; protected internal set; }

    public static UserEntity CreateUser(string login, string password, string name)
    {
        
        
        return new UserEntity
        {
            Login = login,
            PasswordHash = null,
            Name = name
        };
    }
}