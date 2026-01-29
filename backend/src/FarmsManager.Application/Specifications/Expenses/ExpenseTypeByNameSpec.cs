using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class ExpenseTypeByNameSpec : BaseSpecification<ExpenseTypeEntity>,
    ISingleResultSpecification<ExpenseTypeEntity>
{
    public ExpenseTypeByNameSpec(string name)
    {
        Query.Where(t => t.Name == name);
    }
}
