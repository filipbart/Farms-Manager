using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

public class InsertionEntity : Entity
{
    protected InsertionEntity()
    {
        IsSentToIrz = false;
        DateIrzSentUtc = null;
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly InsertionDate { get; init; }
    public Guid HenhouseId { get; init; }
    public Guid HatcheryId { get; init; }
    public int Quantity { get; init; }
    public decimal BodyWeight { get; init; }
    public DateTime? DateIrzSentUtc { get; private set; }
    public bool IsSentToIrz { get; private set; }
    public Guid? SentToIrzBy { get; protected internal set; }

    public virtual CycleEntity Cycle { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }
    public virtual FarmEntity Farm { get; init; }
    public virtual HatcheryEntity Hatchery { get; init; }


    public static InsertionEntity CreateNew(Guid farmId, Guid cycleId, Guid henhouseId, Guid hatcheryId,
        DateOnly insertionDate, int quantity, decimal bodyWeight, Guid? userId = null)
    {
        return new InsertionEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            HenhouseId = henhouseId,
            HatcheryId = hatcheryId,
            InsertionDate = insertionDate,
            Quantity = quantity,
            BodyWeight = bodyWeight,
            CreatedBy = userId
        };
    }

    public void MarkAsSentToIrz(Guid userId)
    {
        IsSentToIrz = true;
        SentToIrzBy = userId;
        DateIrzSentUtc = DateTime.UtcNow;
    }
}