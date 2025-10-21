using System.ComponentModel.DataAnnotations;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;

namespace FarmsManager.Domain.SeedWork;

public abstract class Entity
{
    [Key] public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTime DateCreatedUtc { get; private set; }
    public DateTime? DateModifiedUtc { get; private set; }
    public DateTime? DateDeletedUtc { get; private set; }

    public Guid? CreatedBy { get; set; }
    public virtual UserEntity Creator { get; set; }
    public Guid? ModifiedBy { get; private set; }
    public virtual UserEntity Modifier { get; set; }
    public Guid? DeletedBy { get; private set; }
    public virtual UserEntity Deleter { get; set; }

    protected Entity()
    {
        DateCreatedUtc = DateTime.UtcNow;
    }

    public void SetModified(Guid? modifierId = null)
    {
        DateModifiedUtc = DateTime.UtcNow;
        ModifiedBy = modifierId;
    }

    public void Delete(Guid? deleterId = null, DateTime? dateDeletedUtc = null)
    {
        DateDeletedUtc = dateDeletedUtc ?? DateTime.UtcNow;
        DeletedBy = deleterId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity otherEntity)
            return false;

        return Id == otherEntity.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}