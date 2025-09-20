using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

public class ProductionDataFailureEntity : Entity
{
    protected ProductionDataFailureEntity()
    {
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; protected internal set; }
    public Guid HenhouseId { get; init; }
    public int DeadCount { get; protected internal set; }
    public int DefectiveCount { get; protected internal set; }

    public virtual CycleEntity Cycle { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }
    public virtual FarmEntity Farm { get; init; }

    public static ProductionDataFailureEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        Guid henhouseId,
        int deadCount,
        int defectiveCount,
        Guid? userId = null)
    {
        return new ProductionDataFailureEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            HenhouseId = henhouseId,
            DeadCount = deadCount,
            DefectiveCount = defectiveCount,
            CreatedBy = userId
        };
    }

    public void UpdateData(int deadCount, int defectiveCount)
    {
        DeadCount = deadCount;
        DefectiveCount = defectiveCount;
    }

    public void SetCycle(Guid cycleId)
    {
        CycleId = cycleId;
    }
}