using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

public class ProductionDataWeightStandardEntity : Entity
{
    public int Day { get; protected internal set; }
    public decimal Weight { get; protected internal set; }

    protected ProductionDataWeightStandardEntity()
    {
    }

    public static ProductionDataWeightStandardEntity CreateNew(int day, decimal weight, Guid? userId = null)
    {
        return new ProductionDataWeightStandardEntity
        {
            Day = day,
            Weight = weight,
            CreatedBy = userId
        };
    }

    public void UpdateData(decimal weight)
    {
        Weight = weight;
    }
}