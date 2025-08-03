using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;

namespace FarmsManager.Application.Queries.Sales;

public sealed class GetAllSalesSpec : BaseSpecification<SaleEntity>
{
    public GetAllSalesSpec(GetSalesQueryFilters filters, bool withPagination)
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

    private void PopulateFilters(GetSalesQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.HenhouseIds is not null && filters.HenhouseIds.Count != 0)
        {
            Query.Where(t => filters.HenhouseIds.Contains(t.HenhouseId));
        }

        if (filters.SlaughterhouseIds is not null && filters.SlaughterhouseIds.Count != 0)
        {
            Query.Where(t => filters.SlaughterhouseIds.Contains(t.SlaughterhouseId));
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
            Query.Where(t => t.SaleDate >= filters.DateSince);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(t => t.SaleDate <= filters.DateTo);
        }
    }

    private void ApplyOrdering(GetSalesQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case SaleOrderBy.Cycle:
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

            case SaleOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Farm.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Farm.Name);
                }

                break;

            case SaleOrderBy.Henhouse:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Henhouse.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Henhouse.Name);
                }

                break;

            case SaleOrderBy.SaleDate:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.SaleDate);
                }
                else
                {
                    Query.OrderBy(t => t.SaleDate);
                }

                break;

            case SaleOrderBy.Quantity:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Quantity);
                }
                else
                {
                    Query.OrderBy(t => t.Quantity);
                }

                break;


            case SaleOrderBy.Weight:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Weight);
                }
                else
                {
                    Query.OrderBy(t => t.Weight);
                }

                break;
            case SaleOrderBy.DateCreatedUtc:
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