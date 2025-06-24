using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

public class InsertionEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly InsertionDate { get; init; }
    public Guid HenhouseId { get; init; }
    public Guid HatcheryId { get; init; }
    public int Quantity { get; init; }
    public decimal BodyWeight { get; init; }

    public virtual CycleEntity Cycle { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }


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
}