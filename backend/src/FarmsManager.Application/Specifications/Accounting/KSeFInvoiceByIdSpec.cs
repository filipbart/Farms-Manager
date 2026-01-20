using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.Accounting;

public sealed class KSeFInvoiceByIdSpec : BaseSpecification<KSeFInvoiceEntity>,
    ISingleResultSpecification<KSeFInvoiceEntity>
{
    public KSeFInvoiceByIdSpec(Guid id)
    {
        DisableTracking();
        Query.Where(i => i.Id == id);
    }
}
