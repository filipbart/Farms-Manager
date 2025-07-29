using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

public class ProductionDataTransferFeedEntity : Entity
{
    protected ProductionDataTransferFeedEntity()
    {
    }

    public Guid FromFarmId { get; init; }
    public Guid FromCycleId { get; init; }
    public Guid FromHenhouseId { get; init; }

    public Guid ToFarmId { get; init; }
    public Guid ToCycleId { get; init; }
    public Guid ToHenhouseId { get; init; }

    public string FeedName { get; init; }
    public int RemainingTonnage { get; protected internal set; }
    public decimal RemainingValue { get; protected internal set; }

    public virtual CycleEntity FromCycle { get; init; }
    public virtual HenhouseEntity FromHenhouse { get; init; }
    public virtual FarmEntity FromFarm { get; init; }

    public virtual CycleEntity ToCycle { get; init; }
    public virtual HenhouseEntity ToHenhouse { get; init; }
    public virtual FarmEntity ToFarm { get; init; }

    public static ProductionDataTransferFeedEntity CreateNew(
        Guid fromFarmId,
        Guid fromCycleId,
        Guid fromHenhouseId,
        Guid toFarmId,
        Guid toCycleId,
        Guid toHenhouseId,
        string feedName,
        int remainingTonnage,
        decimal remainingValue,
        Guid? userId = null)
    {
        return new ProductionDataTransferFeedEntity
        {
            FromFarmId = fromFarmId,
            FromCycleId = fromCycleId,
            FromHenhouseId = fromHenhouseId,
            ToFarmId = toFarmId,
            ToCycleId = toCycleId,
            ToHenhouseId = toHenhouseId,
            FeedName = feedName,
            RemainingTonnage = remainingTonnage,
            RemainingValue = remainingValue,
            CreatedBy = userId
        };
    }

    public void UpdateData(int remainingTonnage, decimal remainingValue)
    {
        RemainingTonnage = remainingTonnage;
        RemainingValue = remainingValue;
    }
}