using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

public class FeedPriceEntity : Entity
{
    protected FeedPriceEntity()
    {
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly PriceDate { get; protected internal set; }
    public string Name { get; protected internal set; }
    public decimal Price { get; protected internal set; }
    public virtual CycleEntity Cycle { get; init; }
    public virtual FarmEntity Farm { get; init; }

    public static FeedPriceEntity CreateNew(Guid farmId, Guid cycleId, DateOnly priceDate, string name, decimal price,
        Guid? userId = null)
    {
        return new FeedPriceEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            PriceDate = priceDate,
            Name = name,
            Price = price,
            CreatedBy = userId
        };
    }
}