using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetFeedPriceForFeedInvoiceSpec : BaseSpecification<FeedPriceEntity>,
    ISingleResultSpecification<FeedPriceEntity>
{
    public GetFeedPriceForFeedInvoiceSpec(Guid farmId, string feedName, DateOnly invoiceDate)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.FarmId == farmId);
        Query.Where(t => t.Name == feedName);
        Query.Where(t => t.PriceDate <= invoiceDate);
    }
}