using Ardalis.Specification;
using FarmsManager.Application.Queries.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetAllFeedsDeliveriesSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetAllFeedsDeliveriesSpec(GetFeedsDeliveriesQueryFilters filters, List<Guid> accessibleFarmIds)
    {
        EnsureExists();
        DisableTracking();

        PopulateFilters(filters);

        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        Query.OrderByDescending(t => t.DateCreatedUtc);
    }

    private void PopulateFilters(GetFeedsDeliveriesQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<FeedInvoiceEntity>();

            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
        }

        if (filters.IncorrectPrices.HasValue)
        {
            Query.Where(t => t.CorrectUnitPrice.HasValue == filters.IncorrectPrices.Value);
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
}