using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Queries.ProductionData.Weighings;

public sealed class GetAllProductionDataWeighingsSpec : BaseSpecification<ProductionDataWeighingEntity>
{
    public GetAllProductionDataWeighingsSpec(GetProductionDataWeighingsQueryFilters filters, bool withPagination, List<Guid> accessibleFarmIds, bool isAdmin)
    {
        EnsureExists(filters.ShowDeleted, isAdmin);
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Henhouse);
        Query.Include(t => t.Cycle);
        Query.Include(t => t.Hatchery);
        
        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        PopulateFilters(filters);
        ApplyOrdering(filters);
        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetProductionDataWeighingsQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.HenhouseIds is not null && filters.HenhouseIds.Count != 0)
        {
            Query.Where(t => filters.HenhouseIds.Contains(t.HenhouseId));
        }

        if (filters.HatcheryIds is not null && filters.HatcheryIds.Count != 0)
        {
            Query.Where(t => filters.HatcheryIds.Contains(t.HatcheryId));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<ProductionDataWeighingEntity>();

            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
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

    private void ApplyOrdering(GetProductionDataWeighingsQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case ProductionDataWeighingsOrderBy.Cycle:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Cycle.Identifier).ThenByDescending(t => t.Cycle.Year);
                }
                else
                {
                    Query.OrderBy(t => t.Cycle.Year).ThenBy(t => t.Cycle.Identifier);
                }

                break;

            case ProductionDataWeighingsOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Farm.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Farm.Name);
                }

                break;

            case ProductionDataWeighingsOrderBy.Henhouse:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Henhouse.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Henhouse.Name);
                }

                break;

            case ProductionDataWeighingsOrderBy.Hatchery:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Hatchery.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Hatchery.Name);
                }

                break;

            case ProductionDataWeighingsOrderBy.Weighing1Weight:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Weighing1Weight);
                }
                else
                {
                    Query.OrderBy(t => t.Weighing1Weight);
                }

                break;

            case ProductionDataWeighingsOrderBy.Weighing2Weight:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Weighing2Weight);
                }
                else
                {
                    Query.OrderBy(t => t.Weighing2Weight);
                }

                break;

            case ProductionDataWeighingsOrderBy.Weighing3Weight:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Weighing3Weight);
                }
                else
                {
                    Query.OrderBy(t => t.Weighing3Weight);
                }

                break;

            case ProductionDataWeighingsOrderBy.Weighing4Weight:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Weighing4Weight);
                }
                else
                {
                    Query.OrderBy(t => t.Weighing4Weight);
                }

                break;

            case ProductionDataWeighingsOrderBy.Weighing5Weight:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Weighing5Weight);
                }
                else
                {
                    Query.OrderBy(t => t.Weighing5Weight);
                }

                break;

            case ProductionDataWeighingsOrderBy.DateCreatedUtc:
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