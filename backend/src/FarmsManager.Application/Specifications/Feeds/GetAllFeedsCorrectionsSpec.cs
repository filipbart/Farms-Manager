using Ardalis.Specification;
using FarmsManager.Application.Queries.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetAllFeedsCorrectionsSpec : BaseSpecification<FeedInvoiceCorrectionEntity>
{
    public GetAllFeedsCorrectionsSpec(GetFeedsCorrectionsQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        ApplyOrdering(filters);
        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void ApplyOrdering(GetFeedsCorrectionsQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case FeedsCorrectionsOrderBy.DateCreatedUtc:
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