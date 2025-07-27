using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class GetExpenseProductionByIdSpec : BaseSpecification<ExpenseProductionEntity>,
    ISingleResultSpecification<ExpenseProductionEntity>
{
    public GetExpenseProductionByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}