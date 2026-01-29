using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Queries.Accounting.InvoiceModuleAssignmentRules;

public sealed class GetInvoiceModuleAssignmentRulesSpec : BaseSpecification<InvoiceModuleAssignmentRuleEntity>
{
    public GetInvoiceModuleAssignmentRulesSpec()
    {
        EnsureExists();
        DisableTracking();

        Query.Include(x => x.TaxBusinessEntity);
        Query.Include(x => x.Farm);

        Query.OrderBy(x => x.Priority);
    }
}
