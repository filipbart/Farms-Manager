using Ardalis.GuardClauses;
using FarmsManager.Domain.Aggregates.UserAggregate.Models;
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
    public IrzplusCredentials IrzplusCredentials { get; protected internal set; }
    public bool IsAdmin { get; protected internal set; }


    private readonly List<UserPermissionEntity> _permissions = new();
    private readonly List<UserFarmEntity> _farms = new();
    public virtual IReadOnlyCollection<UserPermissionEntity> Permissions => _permissions;
    public virtual IReadOnlyCollection<UserFarmEntity> Farms => _farms;

    public static UserEntity CreateUser(string login, string name, Guid? userId = null)
    {
        var user = new UserEntity
        {
            Login = login,
            CreatedBy = userId
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

    public void ChangeIrzplusCredentials(IrzplusCredentials irzplusCredentials)
    {
        Guard.Against.Null(irzplusCredentials);
        IrzplusCredentials = irzplusCredentials;
    }

    public IEnumerable<string> GetPermissions()
    {
        return Permissions.Select(t => t.PermissionName);
    }

    public void AddPermission(string permission, Guid? creator)
    {
        Guard.Against.NullOrWhiteSpace(permission, nameof(permission));

        _permissions.Add(new UserPermissionEntity
        {
            CreatedBy = creator,
            UserId = Id,
            PermissionName = permission
        });
    }

    public void RemovePermission(string permission)
    {
        Guard.Against.NullOrWhiteSpace(permission, nameof(permission));

        var permissionData = Permissions.FirstOrDefault(t => t.PermissionName == permission);
        if (permissionData is not null)
        {
            _permissions.Remove(permissionData);
        }
    }

    public void AddFarm(Guid farmId)
    {
        _farms.Add(new UserFarmEntity
        {
            FarmId = farmId,
            UserId = Id
        });
    }

    public void RemoveFarm(Guid farmId)
    {
        var farm = _farms.FirstOrDefault(t => t.FarmId == farmId);
        _farms.Remove(farm);
    }
}