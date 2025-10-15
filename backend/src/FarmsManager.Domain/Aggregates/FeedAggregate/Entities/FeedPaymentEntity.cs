using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Enums;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

public class FeedPaymentEntity : Entity
{
    protected FeedPaymentEntity()
    {
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public string FileName { get; init; }
    public string FilePath { get; init; }
    public FeedPaymentStatus Status { get; protected internal set; }
    public string Comment { get; protected internal set; }
    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }

    public static FeedPaymentEntity CreateNew(Guid farmId, Guid cycleId, string fileName, string filePath,
        Guid? userId = null)
    {
        return new FeedPaymentEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            FileName = fileName,
            FilePath = filePath,
            Status = FeedPaymentStatus.Unrealized,
            CreatedBy = userId
        };
    }

    public void MarkAsCompleted(string comment = null)
    {
        Status = FeedPaymentStatus.Realized;
        Comment = comment;
    }
}