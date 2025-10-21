using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Queries.Sales;

public sealed class GetAllSalesSpec : BaseSpecification<SaleEntity>
{
    public GetAllSalesSpec(GetSalesQueryFilters filters, bool withPagination, List<Guid> accessibleFarmIds, bool isAdmin)
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

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<SaleEntity>();

            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
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