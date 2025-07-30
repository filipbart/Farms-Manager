using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

public class ProductionDataRemainingFeedEntity : Entity
{
    protected ProductionDataRemainingFeedEntity()
    {
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid HenhouseId { get; init; }
    public string FeedName { get; init; }
    public decimal RemainingTonnage { get; protected internal set; }
    public decimal RemainingValue { get; protected internal set; }

    public virtual CycleEntity Cycle { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }
    public virtual FarmEntity Farm { get; init; }

    public static ProductionDataRemainingFeedEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        Guid henhouseId,
        string feedName,
        decimal remainingTonnage,
        decimal remainingValue,
        Guid? userId = null)
    {
        return new ProductionDataRemainingFeedEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            HenhouseId = henhouseId,
            FeedName = feedName,
            RemainingTonnage = remainingTonnage,
            RemainingValue = remainingValue,
            CreatedBy = userId
        };
    }

    public void UpdateData(decimal remainingTonnage, decimal remainingValue)
    {
        RemainingTonnage = remainingTonnage;
        RemainingValue = remainingValue;
    }
}