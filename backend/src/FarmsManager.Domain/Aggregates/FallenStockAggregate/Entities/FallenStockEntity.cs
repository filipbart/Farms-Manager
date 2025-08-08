using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;

public class FallenStockEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid UtilizationPlantId { get; init; }
    public Guid HenhouseId { get; init; }
    public DateOnly Date { get; protected internal set; }
    public int Quantity { get; protected internal set; }
    public DateTime? DateIrzSentUtc { get; protected internal set; }
    public Guid? SentToIrzBy { get; protected internal set; }
    public string DocumentNumber { get; protected internal set; }

    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }
    public virtual UtilizationPlantEntity UtilizationPlant { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }

    public static FallenStockEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        Guid utilizationPlantId,
        Guid henhouseId,
        DateOnly date,
        int quantity,
        Guid? userId = null)
    {
        return new FallenStockEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            UtilizationPlantId = utilizationPlantId,
            HenhouseId = henhouseId,
            Date = date,
            Quantity = quantity,
            CreatedBy = userId
        };
    }

    public void Update(DateOnly date, int quantity)
    {
        Date = date;
        Quantity = quantity;
    }

    public void MarkAsSentToIrz(string documentNumber, Guid userId)
    {
        DateIrzSentUtc = DateTime.UtcNow;
        SentToIrzBy = userId;
        DocumentNumber = documentNumber;
    }

    public bool IsAlreadySentToIrz() => DateIrzSentUtc.HasValue;
}