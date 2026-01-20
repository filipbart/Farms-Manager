using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.Accounting;

public sealed class KSeFInvoiceAttachmentByIdSpec : BaseSpecification<KSeFInvoiceAttachmentEntity>,
    ISingleResultSpecification<KSeFInvoiceAttachmentEntity>
{
    public KSeFInvoiceAttachmentByIdSpec(Guid id)
    {
        DisableTracking();
        EnsureExists();
        Query.Where(a => a.Id == id);
    }
}
