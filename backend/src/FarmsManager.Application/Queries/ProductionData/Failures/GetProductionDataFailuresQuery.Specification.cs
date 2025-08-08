using Ardalis.Specification;
using FarmsManager.Application.Models.ProductionData;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

namespace FarmsManager.Application.Queries.ProductionData.Failures;

public sealed class GetAllProductionDataFailuresSpec : BaseSpecification<ProductionDataFailureEntity>
{
    public GetAllProductionDataFailuresSpec(ProductionDataQueryFilters filters, bool withPagination)
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

    private void PopulateFilters(ProductionDataQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Any())
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.HenhouseIds is not null && filters.HenhouseIds.Any())
        {
            Query.Where(t => filters.HenhouseIds.Contains(t.HenhouseId));
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
            var dateSince = filters.DateSince.Value.ToDateTime(TimeOnly.MinValue);
            Query.Where(t => t.DateCreatedUtc >= dateSince);
        }

        if (filters.DateTo is not null)
        {
            var dateTo = filters.DateTo.Value.ToDateTime(TimeOnly.MaxValue);
            Query.Where(t => t.DateCreatedUtc <= dateTo);
        }
    }

    private void ApplyOrdering(ProductionDataQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case ProductionDataOrderBy.Cycle:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Cycle.Identifier)
                        .ThenByDescending(t => t.Cycle.Year);
                }
                else
                {
                    Query.OrderBy(t => t.Cycle.Year)
                        .ThenBy(t => t.Cycle.Identifier);
                }

                break;

            case ProductionDataOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Farm.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Farm.Name);
                }

                break;

            case ProductionDataOrderBy.Henhouse:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Henhouse.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Henhouse.Name);
                }

                break;

            case ProductionDataOrderBy.DeadCount:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.DeadCount);
                }
                else
                {
                    Query.OrderBy(t => t.DeadCount);
                }

                break;

            case ProductionDataOrderBy.DefectiveCount:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.DefectiveCount);
                }
                else
                {
                    Query.OrderBy(t => t.DefectiveCount);
                }

                break;

            case ProductionDataOrderBy.DateCreatedUtc:
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