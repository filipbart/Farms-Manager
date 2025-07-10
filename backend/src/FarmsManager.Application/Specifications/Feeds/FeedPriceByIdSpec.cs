using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class FeedPriceByIdSpec : BaseSpecification<FeedPriceEntity>, ISingleResultSpecification<FeedPriceEntity>
{
    public FeedPriceByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}