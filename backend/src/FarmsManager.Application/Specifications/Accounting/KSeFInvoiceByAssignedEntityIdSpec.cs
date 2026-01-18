using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Commands.Accounting;

public sealed class KSeFInvoiceByAssignedEntityIdSpec : BaseSpecification<KSeFInvoiceEntity>,
    ISingleResultSpecification<KSeFInvoiceEntity>
{
    public KSeFInvoiceByAssignedEntityIdSpec(Guid entityId)
    {
        Query
            .Where(t => t.AssignedEntityInvoiceId == entityId && t.DateDeletedUtc == null);
    }
}
