using Ardalis.Specification;
using FarmsManager.Application.Queries.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetAllFeedsDeliveriesSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetAllFeedsDeliveriesSpec(GetFeedsDeliveriesQueryFilters filters, bool withPagination)
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

            case FeedsDeliveriesOrderBy.ItemName:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.ItemName);
                }
                else
                {
                    Query.OrderBy(t => t.ItemName);
                }

                break;

            case FeedsDeliveriesOrderBy.VendorName:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.VendorName);
                }
                else
                {
                    Query.OrderBy(t => t.VendorName);
                }

                break;

            case FeedsDeliveriesOrderBy.UnitPrice:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.CorrectUnitPrice.HasValue)
                        .ThenByDescending(t => t.CorrectUnitPrice.Value).ThenByDescending(t => t.UnitPrice);
                }
                else
                {
                    Query.OrderByDescending(t => t.CorrectUnitPrice.HasValue)
                        .ThenBy(t => t.CorrectUnitPrice.Value).ThenBy(t => t.UnitPrice);
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