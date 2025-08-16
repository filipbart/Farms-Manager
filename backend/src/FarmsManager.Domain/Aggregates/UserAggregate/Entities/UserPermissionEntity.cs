using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.UserAggregate.Entities;

public class UserPermissionEntity : Entity
{
    public Guid UserId { get; init; }
    public string PermissionName { get; init; }
    public virtual UserEntity User { get; set; }
}