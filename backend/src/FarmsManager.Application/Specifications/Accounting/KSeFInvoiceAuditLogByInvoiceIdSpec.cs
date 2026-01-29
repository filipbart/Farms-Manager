using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.Accounting;

public sealed class KSeFInvoiceAuditLogByInvoiceIdSpec : BaseSpecification<KSeFInvoiceAuditLogEntity>
{
    public KSeFInvoiceAuditLogByInvoiceIdSpec(Guid invoiceId)
    {
        DisableTracking();
        EnsureExists();
        Query.Where(a => a.InvoiceId == invoiceId)
             .OrderByDescending(a => a.DateCreatedUtc);
    }
}
