using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class FeedInvoicesByPaymentIdSpec : Specification<FeedInvoiceEntity>
{
    public FeedInvoicesByPaymentIdSpec(Guid paymentId)
    {
        Query
            .Where(t => t.PaymentId == paymentId && t.DateDeletedUtc == null);
    }
}
