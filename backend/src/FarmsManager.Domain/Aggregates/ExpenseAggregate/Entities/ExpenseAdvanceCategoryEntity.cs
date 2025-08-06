using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseAdvanceCategoryEntity : Entity
{
    public ExpenseAdvanceCategoryType Type { get; init; }
    public string Name { get; init; }

    protected ExpenseAdvanceCategoryEntity()
    {
    }

    public static ExpenseAdvanceCategoryEntity CreateNew(string name, ExpenseAdvanceCategoryType type,
        Guid? userId = null)
    {
        return new ExpenseAdvanceCategoryEntity
        {
            Type = type,
            Name = name,
            CreatedBy = userId
        };
    }
}