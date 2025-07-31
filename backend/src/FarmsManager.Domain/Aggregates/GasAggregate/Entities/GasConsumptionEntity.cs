using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.GasAggregate.Entities;

public class GasConsumptionEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public decimal QuantityConsumed { get; protected internal set; }
    public decimal Cost { get; protected internal set; }

    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }

    public static GasConsumptionEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        decimal quantityConsumed,
        decimal cost,
        Guid? userId = null)
    {
        return new GasConsumptionEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            QuantityConsumed = quantityConsumed,
            Cost = cost,
            CreatedBy = userId
        };
    }

    public void Update(
        decimal quantityConsumed,
        decimal cost)
    {
        QuantityConsumed = quantityConsumed;
        Cost = cost;
    }
}