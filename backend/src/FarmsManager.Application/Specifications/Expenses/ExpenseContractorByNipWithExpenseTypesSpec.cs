using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class ExpenseContractorByNipWithExpenseTypesSpec : BaseSpecification<ExpenseContractorEntity>,
    ISingleResultSpecification<ExpenseContractorEntity>
{
    public ExpenseContractorByNipWithExpenseTypesSpec(string nip)
    {
        nip = nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();

        EnsureExists();
        Query.Where(t => t.Nip == nip)
             .Include(t => t.ExpenseTypes);
    }
}
