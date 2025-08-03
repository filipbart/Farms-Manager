using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class ExpenseContractorByNipSpec : BaseSpecification<ExpenseContractorEntity>,
    ISingleResultSpecification<ExpenseContractorEntity>
{
    public ExpenseContractorByNipSpec(string nip)
    {
        nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.Nip == nip);
    }
}