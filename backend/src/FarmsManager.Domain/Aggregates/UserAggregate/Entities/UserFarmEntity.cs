using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.UserAggregate.Entities;

public class UserFarmEntity : Entity
{
    public Guid UserId { get; init; }
    public Guid FarmId { get; init; }
    public virtual UserEntity User { get; init; }
    public virtual FarmEntity Farm { get; init; }
}