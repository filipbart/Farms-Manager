using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Queries.ProductionData.FlockLosses;

public sealed class GetAllProductionDataFlockLossesSpec : BaseSpecification<ProductionDataFlockLossMeasureEntity>
{
    public GetAllProductionDataFlockLossesSpec(GetProductionDataFlockLossesQueryFilters filters, bool withPagination,
        List<Guid> accessibleFarmIds, bool isAdmin)
    {
        EnsureExists(filters.ShowDeleted, isAdmin);
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Henhouse);
        Query.Include(t => t.Cycle);
        Query.Include(t => t.Hatchery);
        Query.Include(t => t.Insertion);

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

    private void PopulateFilters(GetProductionDataFlockLossesQueryFilters filters)
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
            var predicate = PredicateBuilder.New<ProductionDataFlockLossMeasureEntity>();

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

    private void ApplyOrdering(GetProductionDataFlockLossesQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case ProductionDataFlockLossesOrderBy.Cycle:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Cycle.Identifier).ThenByDescending(t => t.Cycle.Year);
                }
                else
                {
                    Query.OrderBy(t => t.Cycle.Year).ThenBy(t => t.Cycle.Identifier);
                }

                break;

            case ProductionDataFlockLossesOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Farm.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Farm.Name);
                }

                break;

            case ProductionDataFlockLossesOrderBy.Henhouse:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Henhouse.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Henhouse.Name);
                }

                break;

            case ProductionDataFlockLossesOrderBy.Hatchery:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Hatchery.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Hatchery.Name);
                }

                break;

            case ProductionDataFlockLossesOrderBy.FlockLoss1Percentage:
                if (isDescending)
                {
                    Query.OrderByDescending(t => (decimal?)t.FlockLoss1Quantity / t.Insertion.Quantity);
                }
                else
                {
                    Query.OrderBy(t => (decimal?)t.FlockLoss1Quantity / t.Insertion.Quantity);
                }

                break;

            case ProductionDataFlockLossesOrderBy.FlockLoss2Percentage:
                if (isDescending)
                {
                    Query.OrderByDescending(t => (decimal?)t.FlockLoss2Quantity / t.Insertion.Quantity);
                }
                else
                {
                    Query.OrderBy(t => (decimal?)t.FlockLoss2Quantity / t.Insertion.Quantity);
                }

                break;

            case ProductionDataFlockLossesOrderBy.FlockLoss3Percentage:
                if (isDescending)
                {
                    Query.OrderByDescending(t => (decimal?)t.FlockLoss3Quantity / t.Insertion.Quantity);
                }
                else
                {
                    Query.OrderBy(t => (decimal?)t.FlockLoss3Quantity / t.Insertion.Quantity);
                }

                break;

            case ProductionDataFlockLossesOrderBy.FlockLoss4Percentage:
                if (isDescending)
                {
                    Query.OrderByDescending(t => (decimal?)t.FlockLoss4Quantity / t.Insertion.Quantity);
                }
                else
                {
                    Query.OrderBy(t => (decimal?)t.FlockLoss4Quantity / t.Insertion.Quantity);
                }

                break;

            case ProductionDataFlockLossesOrderBy.DateCreatedUtc:
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