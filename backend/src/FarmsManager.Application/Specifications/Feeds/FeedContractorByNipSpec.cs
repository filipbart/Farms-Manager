using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class FeedContractorByNipSpec : BaseSpecification<FeedContractorEntity>,
    ISingleResultSpecification<FeedContractorEntity>
{
    public FeedContractorByNipSpec(string nip)
    {
        nip = nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.Nip == nip);
    }
}
