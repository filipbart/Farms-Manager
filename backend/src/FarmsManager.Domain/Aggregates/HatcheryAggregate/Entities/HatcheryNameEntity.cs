using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

public class HatcheryNameEntity : Entity
{
    protected HatcheryNameEntity()
    {
    }

    public string Name { get; init; }

    public static HatcheryNameEntity CreateNew(string name, Guid? userId = null)
    {
        return new HatcheryNameEntity
        {
            Name = name,
            CreatedBy = userId
        };
    }
}