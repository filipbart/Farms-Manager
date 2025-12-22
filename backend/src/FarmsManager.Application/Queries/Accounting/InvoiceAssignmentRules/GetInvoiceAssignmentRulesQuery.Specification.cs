using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Queries.Accounting.InvoiceAssignmentRules;

public sealed class GetInvoiceAssignmentRulesSpec : BaseSpecification<InvoiceAssignmentRuleEntity>
{
    public GetInvoiceAssignmentRulesSpec()
    {
        EnsureExists();
        DisableTracking();

        Query.Include(x => x.AssignedUser);
        Query.Include(x => x.TaxBusinessEntity);
        Query.Include(x => x.Farm);

        Query.OrderBy(x => x.Priority);
    }
}
