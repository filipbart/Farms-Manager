using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class ExpenseContractorByNameWithExpenseTypesSpec : BaseSpecification<ExpenseContractorEntity>,
    ISingleResultSpecification<ExpenseContractorEntity>
{
    public ExpenseContractorByNameWithExpenseTypesSpec(string name)
    {
        EnsureExists();
        Query.Where(t => t.Name == name)
             .Include(t => t.ExpenseTypes);
    }
}
