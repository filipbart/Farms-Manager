using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

public class CycleEntity : Entity
{
    public Guid FarmId { get; init; }
    public int Identifier { get; init; }
    public int Year { get; init; }
    public virtual FarmEntity Farm { get; init; }

    public static CycleEntity CreateNew(int identifier, int year, Guid farmId, Guid? userId = null)
    {
        return new CycleEntity
        {
            Identifier = identifier,
            Year = year,
            FarmId = farmId,
            CreatedBy = userId
        };
    }
}