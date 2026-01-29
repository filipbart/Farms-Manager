using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Queries.Accounting.InvoiceFarmAssignmentRules;

public class GetInvoiceFarmAssignmentRulesSpec : BaseSpecification<InvoiceFarmAssignmentRuleEntity>
{
    public GetInvoiceFarmAssignmentRulesSpec()
    {
        EnsureExists();
        Query
            .Include(r => r.TargetFarm)
            .Include(r => r.TaxBusinessEntity)
            .OrderBy(r => r.Priority);
    }
}
