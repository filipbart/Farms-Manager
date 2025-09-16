using FarmsManager.Domain.Aggregates.FallenStockAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;

public class FallenStockEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid? UtilizationPlantId { get; init; }
    public Guid HenhouseId { get; init; }
    public FallenStockType Type { get; init; }
    public DateOnly Date { get; protected internal set; }
    public int Quantity { get; protected internal set; }
    public DateTime? DateIrzSentUtc { get; protected internal set; }
    public Guid? SentToIrzBy { get; protected internal set; }
    public string DocumentNumber { get; protected internal set; }
    public Guid InternalGroupId { get; init; }

    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }
    public virtual UtilizationPlantEntity UtilizationPlant { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }

    public static FallenStockEntity CreateNew(
        Guid internalGroupId,
        FarmEntity farm,
        CycleEntity cycle,
        UtilizationPlantEntity utilizationPlant,
        HenhouseEntity henhouse,
        FallenStockType type,
        DateOnly date,
        int quantity,
        Guid? userId = null)
    {
        return new FallenStockEntity
        {
            InternalGroupId = internalGroupId,
            FarmId = farm.Id,
            Farm = farm,
            CycleId = cycle.Id,
            Cycle = cycle,
            UtilizationPlantId = utilizationPlant?.Id,
            UtilizationPlant = utilizationPlant,
            HenhouseId = henhouse.Id,
            Henhouse = henhouse,
            Type = type,
            Date = date,
            Quantity = quantity,
            CreatedBy = userId
        };
    }

    public void Update(int quantity)
    {
        Quantity = quantity;
    }

    public void MarkAsSentToIrz(string documentNumber, Guid userId)
    {
        DateIrzSentUtc = DateTime.UtcNow;
        SentToIrzBy = userId;
        DocumentNumber = documentNumber;
    }
}