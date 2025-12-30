using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class ExpenseContractorByNameSpec : BaseSpecification<ExpenseContractorEntity>,
    ISingleResultSpecification<ExpenseContractorEntity>
{
    public ExpenseContractorByNameSpec(string name)
    {
        DisableTracking();

        Query.Where(t => t.Name.ToLower() == name.ToLower());
    }
}
