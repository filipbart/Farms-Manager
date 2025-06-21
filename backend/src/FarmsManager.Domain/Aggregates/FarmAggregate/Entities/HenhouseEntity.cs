using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

public class HenhouseEntity : Entity
{
    protected HenhouseEntity()
    {
    }

    public Guid FarmId { get; init; }
    public string Name { get; protected internal set; }
    public int Number { get; protected internal set; }
    public string Description { get; protected internal set; }
    public virtual FarmEntity Farm { get; init; }
}