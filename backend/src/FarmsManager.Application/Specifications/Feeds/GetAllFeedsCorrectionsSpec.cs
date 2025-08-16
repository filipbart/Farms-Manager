using Ardalis.Specification;
using FarmsManager.Application.Queries.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetAllFeedsCorrectionsSpec : BaseSpecification<FeedInvoiceCorrectionEntity>
{
    public GetAllFeedsCorrectionsSpec(GetFeedsDeliveriesQueryFilters filters, bool withPagination,
        List<Guid> accessibleFarmIds)
    {
        EnsureExists();
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

    private void PopulateFilters(GetFeedsDeliveriesQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<FeedInvoiceCorrectionEntity>();

            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
        }

        if (filters.DateSince is not null)
        {
            Query.Where(t => t.InvoiceDate >= filters.DateSince);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(t => t.InvoiceDate <= filters.DateTo);
        }
    }

    private void ApplyOrdering(GetFeedsDeliveriesQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case FeedsDeliveriesOrderBy.Cycle:
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

            case FeedsDeliveriesOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Farm.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Farm.Name);
                }

                break;

            case FeedsDeliveriesOrderBy.DateCreatedUtc:
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