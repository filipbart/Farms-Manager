using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class GetAllExpenseContractorsSpec : BaseSpecification<ExpenseContractorEntity>
{
    public GetAllExpenseContractorsSpec()
    {
        EnsureExists();
        Query.OrderBy(t => t.Name);
    }
}