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
    public DateOnly InsertionDate { get; protected internal set; }
    public Guid HenhouseId { get; init; }
    public Guid HatcheryId { get; init; }
    public int Quantity { get; protected internal set; }
    public decimal BodyWeight { get; protected internal set; }
    public Guid InternalGroupId { get; init; }
    public DateTime? DateIrzSentUtc { get; private set; }
    public bool IsSentToIrz { get; private set; }
    public Guid? SentToIrzBy { get; protected internal set; }
    public string DocumentNumber { get; protected internal set; }
    public string IrzComment { get; protected internal set; }

    public bool ReportedToWios { get; protected internal set; }
    public string WiosComment { get; protected internal set; }

    public virtual CycleEntity Cycle { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }
    public virtual FarmEntity Farm { get; init; }
    public virtual HatcheryEntity Hatchery { get; init; }


    public static InsertionEntity CreateNew(Guid internalGroupId, Guid farmId, Guid cycleId, Guid henhouseId,
        Guid hatcheryId,
        DateOnly insertionDate, int quantity, decimal bodyWeight, Guid? userId = null)
    {
        return new InsertionEntity
        {
            InternalGroupId = internalGroupId,
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

    public void MarkAsSentToIrz(string documentNumber, Guid userId, string comment = null)
    {
        IsSentToIrz = true;
        SentToIrzBy = userId;
        DateIrzSentUtc = DateTime.UtcNow;
        DocumentNumber = documentNumber;
        IrzComment = comment;
    }

    public void MarkAsSentToIrzByAdmin(Guid userId)
    {
        IsSentToIrz = true;
        SentToIrzBy = userId;
    }

    public void UpdateData(DateOnly insertionDate, int quantity, decimal bodyWeight)
    {
        InsertionDate = insertionDate;
        Quantity = quantity;
        BodyWeight = bodyWeight;
    }

    public bool IsAlreadySentToIrz() => DateIrzSentUtc.HasValue || IsSentToIrz;

    public void MarkAsReportedToWios(string comment)
    {
        ReportedToWios = true;
        WiosComment = comment;
    }
}