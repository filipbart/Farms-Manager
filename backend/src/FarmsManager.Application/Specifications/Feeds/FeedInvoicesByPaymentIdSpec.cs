using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class FeedInvoicesByPaymentIdSpec : BaseSpecification<FeedInvoiceEntity>
{
    public FeedInvoicesByPaymentIdSpec(Guid paymentId)
    {
        EnsureExists();
        Query.Where(t => t.PaymentId == paymentId);
    }
}
