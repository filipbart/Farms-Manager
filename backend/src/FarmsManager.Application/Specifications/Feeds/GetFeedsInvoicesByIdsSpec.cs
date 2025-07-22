using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetFeedsInvoicesByIdsSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetFeedsInvoicesByIdsSpec(List<Guid> ids)
    {
        EnsureExists();

        Query.Where(t => ids.Contains(t.Id));
    }
}