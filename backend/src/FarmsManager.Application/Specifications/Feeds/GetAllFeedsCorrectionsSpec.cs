using Ardalis.Specification;
using FarmsManager.Application.Queries.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetAllFeedsCorrectionsSpec : BaseSpecification<FeedInvoiceCorrectionEntity>
{
    public GetAllFeedsCorrectionsSpec(GetFeedsDeliveriesQueryFilters filters, bool withPagination)
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

    private void PopulateFilters(GetFeedsDeliveriesQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
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