using Ardalis.Specification;
using FarmsManager.Application.Queries.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetAllFeedsPaymentsSpec : BaseSpecification<FeedPaymentEntity>
{
    public GetAllFeedsPaymentsSpec(GetFeedsPaymentsQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        ApplyOrdering(filters);
        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void ApplyOrdering(GetFeedsPaymentsQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case FeedsPaymentsOrderBy.DateCreatedUtc:
            default:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.DateCreatedUtc);
                }
                else
                {
                    Query.OrderBy(t => t.DateCreatedUtc);
                }

                break;
        }
    }
}