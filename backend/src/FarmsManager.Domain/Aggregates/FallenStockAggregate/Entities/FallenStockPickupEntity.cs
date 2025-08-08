using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;

public class FallenStockPickupEntity : Entity
{
    protected FallenStockPickupEntity()
    {
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly Date { get; init; }
    public int Quantity { get; protected internal set; }
    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }

    public static FallenStockPickupEntity CreateNew(Guid farmId, Guid cycleId, DateOnly date, int quantity,
        Guid? userId = null)
    {
        return new FallenStockPickupEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            Date = date,
            Quantity = quantity,
            CreatedBy = userId
        };
    }

    public void Update(int quantity)
    {
        Quantity = quantity;
    }
}