using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Queries.Summary.FinancialAnalysis;

public sealed class ExpensesProductionsByFarmsSpec : BaseSpecification<ExpenseProductionEntity>
{
    public ExpensesProductionsByFarmsSpec(List<Guid> farmIds)
    {
        EnsureExists();
        Query.Where(t => farmIds.Contains(t.FarmId));
    }
}