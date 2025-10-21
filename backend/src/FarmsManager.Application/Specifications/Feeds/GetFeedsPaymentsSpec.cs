using Ardalis.Specification;
using FarmsManager.Application.Queries.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetAllFeedsPaymentsSpec : BaseSpecification<FeedPaymentEntity>
{
    public GetAllFeedsPaymentsSpec(GetFeedsPaymentsQueryFilters filters, bool withPagination,
        List<Guid> accessibleFarmIds, bool isAdmin)
    {
        EnsureExists(filters.ShowDeleted, isAdmin);
        DisableTracking();

        PopulateFilters(filters);
        ApplyOrdering(filters);

        Query.Include(t => t.Creator);
        Query.Include(t => t.Modifier);
        Query.Include(t => t.Deleter);

        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetFeedsPaymentsQueryFilters filters)
    {
        if (filters.Status.HasValue)
        {
            Query.Where(t => t.Status == filters.Status.Value);
        }

        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<FeedPaymentEntity>();

            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
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