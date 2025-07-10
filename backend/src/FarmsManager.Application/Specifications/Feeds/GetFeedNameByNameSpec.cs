using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetFeedNameByNameSpec : BaseSpecification<FeedNameEntity>,
    ISingleResultSpecification<FeedNameEntity>
{
    public GetFeedNameByNameSpec(string name)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.Name == name);
    }
}