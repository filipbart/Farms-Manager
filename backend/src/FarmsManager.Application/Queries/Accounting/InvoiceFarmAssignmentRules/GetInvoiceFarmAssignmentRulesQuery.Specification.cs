using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Queries.Accounting.InvoiceFarmAssignmentRules;

public class GetInvoiceFarmAssignmentRulesSpec : Specification<InvoiceFarmAssignmentRuleEntity>
{
    public GetInvoiceFarmAssignmentRulesSpec()
    {
        Query
            .Where(r => r.DateDeletedUtc == null)
            .Include(r => r.TargetFarm)
            .Include(r => r.TaxBusinessEntity)
            .OrderBy(r => r.Priority);
    }
}
