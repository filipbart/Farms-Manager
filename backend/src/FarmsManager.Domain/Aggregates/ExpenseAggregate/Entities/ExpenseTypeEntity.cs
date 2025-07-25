using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseTypeEntity : Entity
{
    protected ExpenseTypeEntity()
    {
    }

    public string Name { get; init; }

    public static ExpenseTypeEntity CreateNew(string name, Guid? userId = null)
    {
        return new ExpenseTypeEntity
        {
            Name = name,
            CreatedBy = userId
        };
    }
}