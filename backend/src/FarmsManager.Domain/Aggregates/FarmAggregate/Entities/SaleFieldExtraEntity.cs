using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

public class SaleFieldExtraEntity : Entity
{
    protected SaleFieldExtraEntity()
    {
    }

    public string Name { get; init; }

    public static SaleFieldExtraEntity CreateNew(string name, Guid? userId = null)
    {
        return new SaleFieldExtraEntity
        {
            Name = name,
            CreatedBy = userId
        };
    }
}