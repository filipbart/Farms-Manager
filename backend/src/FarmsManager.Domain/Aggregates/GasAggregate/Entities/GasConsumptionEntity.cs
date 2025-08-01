using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Enum;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.GasAggregate.Entities;

public class GasConsumptionEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public decimal QuantityConsumed { get; protected internal set; }
    public decimal Cost { get; protected internal set; }

    public GasConsumptionStatus Status { get; private set; }
    public Guid? CorrectionForId { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }

    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }

    public virtual ICollection<GasConsumptionSourceEntity> ConsumptionSources { get; private set; } =
        new List<GasConsumptionSourceEntity>();

    protected GasConsumptionEntity()
    {
    }

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
            Status = GasConsumptionStatus.Active,
            CreatedBy = userId
        };
    }

    public static GasConsumptionEntity CreateCorrection(GasConsumptionEntity original, Guid? userId)
    {
        return new GasConsumptionEntity
        {
            FarmId = original.FarmId,
            CycleId = original.CycleId,
            QuantityConsumed = -original.QuantityConsumed,
            Cost = -original.Cost,
            Status = GasConsumptionStatus.Active,
            CorrectionForId = original.Id,
            CreatedBy = userId
        };
    }

    public void Cancel(Guid? userId)
    {
        if (Status == GasConsumptionStatus.Cancelled)
        {
            throw new Exception("Ten wpis został już anulowany.");
        }

        Status = GasConsumptionStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
        SetModified(userId);
    }
}