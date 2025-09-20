using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

public class ProductionDataTransferFeedEntity : Entity
{
    protected ProductionDataTransferFeedEntity()
    {
    }

    public Guid FromFarmId { get; init; }
    public Guid FromCycleId { get; protected internal set; }
    public Guid FromHenhouseId { get; init; }

    public Guid ToFarmId { get; init; }
    public Guid ToCycleId { get; protected internal set; }
    public Guid ToHenhouseId { get; init; }

    public string FeedName { get; init; }
    public decimal Tonnage { get; protected internal set; }
    public decimal Value { get; protected internal set; }

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
        decimal tonnage,
        decimal value,
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
            Tonnage = tonnage,
            Value = value,
            CreatedBy = userId
        };
    }

    public void UpdateData(decimal tonnage, decimal value)
    {
        Tonnage = tonnage;
        Value = value;
    }

    public void SetFromCycle(Guid fromCycleId)
    {
        FromCycleId = fromCycleId;
    }

    public void SetToCycleId(Guid toCycleId)
    {
        ToCycleId = toCycleId;
    }
}