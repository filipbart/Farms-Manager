using Ardalis.Specification;
using FarmsManager.Application.Queries.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetAllFeedsPricesSpec : BaseSpecification<FeedPriceEntity>
{
    public GetAllFeedsPricesSpec(GetFeedsPricesQueryFilters filters, bool withPagination, List<Guid> accessibleFarmIds, bool isAdmin)
    {
        EnsureExists(filters.ShowDeleted, isAdmin);
        DisableTracking();

        PopulateFilters(filters);
        ApplyOrdering(filters);

        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetFeedsPricesQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.FeedNames is not null && filters.FeedNames.Count != 0)
        {
            Query.Where(t => filters.FeedNames.Contains(t.Name));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<FeedPriceEntity>();

            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
        }

        if (filters.DateSince is not null)
        {
            Query.Where(t => t.PriceDate >= filters.DateSince);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(t => t.PriceDate <= filters.DateTo);
        }
    }

    private void ApplyOrdering(GetFeedsPricesQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case FeedsPricesOrderBy.Cycle:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Cycle.Identifier)
                        .ThenByDescending(t => t.Cycle.Year)
                        .ThenByDescending(t => t.DateCreatedUtc);
                }
                else
                {
                    Query.OrderBy(t => t.Cycle.Year)
                        .ThenBy(t => t.Cycle.Identifier)
                        .ThenBy(t => t.DateCreatedUtc);
                }

                break;

            case FeedsPricesOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Farm.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Farm.Name);
                }

                break;

            case FeedsPricesOrderBy.PriceDate:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.PriceDate);
                }
                else
                {
                    Query.OrderBy(t => t.PriceDate);
                }

                break;

            case FeedsPricesOrderBy.Price:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Price);
                }
                else
                {
                    Query.OrderBy(t => t.Price);
                }

                break;


            case FeedsPricesOrderBy.Name:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Name);
                }

                break;
            case FeedsPricesOrderBy.DateCreatedUtc:
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