using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

public class FeedPaymentEntity : Entity
{
    private FeedPaymentEntity()
    {
    }

    public Guid FarmId { get; init; }
    public string FileName { get; init; }
    public string FilePath { get; init; }
    public virtual FarmEntity Farm { get; init; }

    public static FeedPaymentEntity CreateNew(Guid farmId, string fileName, string filePath, Guid? userId = null)
    {
        return new FeedPaymentEntity
        {
            FarmId = farmId,
            FileName = fileName,
            FilePath = filePath,
            CreatedBy = userId
        };
    }
}