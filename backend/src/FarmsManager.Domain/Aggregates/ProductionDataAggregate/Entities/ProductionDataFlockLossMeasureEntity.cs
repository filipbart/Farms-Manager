using FarmsManager.Domain.SeedWork;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

namespace FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

public class ProductionDataFlockLossMeasureEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid HenhouseId { get; init; }
    public Guid CycleId { get; init; }
    public Guid HatcheryId { get; init; }
    public Guid InsertionId { get; init; }

    // Pomiar I
    public int? FlockLoss1Day { get; private set; }
    public int? FlockLoss1Quantity { get; private set; }

    // Pomiar II
    public int? FlockLoss2Day { get; private set; }
    public int? FlockLoss2Quantity { get; private set; }

    // Pomiar III
    public int? FlockLoss3Day { get; private set; }
    public int? FlockLoss3Quantity { get; private set; }

    // Pomiar IV
    public int? FlockLoss4Day { get; private set; }
    public int? FlockLoss4Quantity { get; private set; }

    public virtual FarmEntity Farm { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }
    public virtual CycleEntity Cycle { get; init; }
    public virtual HatcheryEntity Hatchery { get; init; }
    public virtual InsertionEntity Insertion { get; init; }

    protected ProductionDataFlockLossMeasureEntity()
    {
    }

    public static ProductionDataFlockLossMeasureEntity CreateNew(Guid farmId, Guid henhouseId, Guid cycleId,
        Guid hatcheryId, Guid insertionId, int day, int quantity, Guid? userId = null)
    {
        var entity = new ProductionDataFlockLossMeasureEntity
        {
            FarmId = farmId,
            HenhouseId = henhouseId,
            CycleId = cycleId,
            HatcheryId = hatcheryId,
            InsertionId = insertionId,
            CreatedBy = userId
        };

        entity.UpdateMeasure(1, day, quantity);

        return entity;
    }

    public void UpdateMeasure(int measureNumber, int day, int quantity)
    {
        switch (measureNumber)
        {
            case 1:
                FlockLoss1Day = day;
                FlockLoss1Quantity = quantity;
                break;
            case 2:
                FlockLoss2Day = day;
                FlockLoss2Quantity = quantity;
                break;
            case 3:
                FlockLoss3Day = day;
                FlockLoss3Quantity = quantity;
                break;
            case 4:
                FlockLoss4Day = day;
                FlockLoss4Quantity = quantity;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(measureNumber),
                    @"Numer pomiaru musi być pomiędzy 1 a 4.");
        }
    }
}