using FarmsManager.Domain.SeedWork;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

namespace FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

public class ProductionDataWeighingEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid HenhouseId { get; init; }
    public Guid CycleId { get; protected internal set; }
    public Guid HatcheryId { get; init; }


    // Ważenie I
    public int? Weighing1Day { get; private set; }
    public decimal? Weighing1Weight { get; private set; }

    // Ważenie II
    public int? Weighing2Day { get; private set; }
    public decimal? Weighing2Weight { get; private set; }

    // Ważenie III
    public int? Weighing3Day { get; private set; }
    public decimal? Weighing3Weight { get; private set; }

    // Ważenie IV
    public int? Weighing4Day { get; private set; }
    public decimal? Weighing4Weight { get; private set; }

    // Ważenie V
    public int? Weighing5Day { get; private set; }
    public decimal? Weighing5Weight { get; private set; }

    public virtual FarmEntity Farm { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }
    public virtual CycleEntity Cycle { get; init; }
    public virtual HatcheryEntity Hatchery { get; init; }

    protected ProductionDataWeighingEntity()
    {
    }

    public static ProductionDataWeighingEntity CreateNew(Guid farmId, Guid henhouseId, Guid cycleId, Guid hatcheryId,
        int day,
        decimal weight, Guid? userId = null)
    {
        var entity = new ProductionDataWeighingEntity
        {
            FarmId = farmId,
            HenhouseId = henhouseId,
            CycleId = cycleId,
            HatcheryId = hatcheryId,
            CreatedBy = userId
        };

        entity.UpdateWeighing(1, day, weight);

        return entity;
    }

    // Metoda do aktualizacji kolejnych ważeń
    public void UpdateWeighing(int weighingNumber, int day, decimal weight)
    {
        switch (weighingNumber)
        {
            case 1:
                Weighing1Day = day;
                Weighing1Weight = weight;
                break;
            case 2:
                Weighing2Day = day;
                Weighing2Weight = weight;
                break;
            case 3:
                Weighing3Day = day;
                Weighing3Weight = weight;
                break;
            case 4:
                Weighing4Day = day;
                Weighing4Weight = weight;
                break;
            case 5:
                Weighing5Day = day;
                Weighing5Weight = weight;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(weighingNumber),
                    @"Numer ważenia musi być pomiędzy 1 a 5.");
        }
    }

    public void SetCycle(Guid cycleId)
    {
        CycleId = cycleId;
    }
}