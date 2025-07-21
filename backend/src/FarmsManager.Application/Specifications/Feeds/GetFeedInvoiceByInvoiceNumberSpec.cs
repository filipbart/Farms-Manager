using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetFeedInvoiceByInvoiceNumberSpec : BaseSpecification<FeedInvoiceEntity>,
    ISingleResultSpecification<FeedInvoiceEntity>
{
    public GetFeedInvoiceByInvoiceNumberSpec(string invoiceNumber)
    {
        EnsureExists();

        Query.Where(t => t.InvoiceNumber == invoiceNumber);
    }
}