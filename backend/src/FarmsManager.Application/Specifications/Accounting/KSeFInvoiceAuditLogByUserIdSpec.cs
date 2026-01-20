using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.Accounting;

public sealed class KSeFInvoiceAuditLogByUserIdSpec : BaseSpecification<KSeFInvoiceAuditLogEntity>
{
    public KSeFInvoiceAuditLogByUserIdSpec(Guid userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        DisableTracking();
        EnsureExists();
        Query.Where(a => a.UserId == userId);

        if (fromDate.HasValue)
            Query.Where(a => a.DateCreatedUtc >= fromDate.Value);

        if (toDate.HasValue)
            Query.Where(a => a.DateCreatedUtc <= toDate.Value);

        Query.OrderByDescending(a => a.DateCreatedUtc);
    }
}
