using Ardalis.Specification;
using FarmsManager.Application.Queries.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetAllFeedsPricesSpec : BaseSpecification<FeedPriceEntity>
{
    public GetAllFeedsPricesSpec(GetFeedsPricesQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        PopulateFilters(filters);
        ApplyOrdering(filters);
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

        if (filters.Cycles is not null && filters.Cycles.Count != 0)
        {
            var validPairs = filters.Cycles
                .Select(c => new { c.Identifier, c.Year })
                .ToList();

            Query.Where(t =>
                validPairs.Any(fc =>
                    fc.Identifier == t.Cycle.Identifier &&
                    fc.Year == t.Cycle.Year));
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