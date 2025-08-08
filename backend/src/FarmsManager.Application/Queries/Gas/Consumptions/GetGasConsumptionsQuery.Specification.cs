using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Enum;

namespace FarmsManager.Application.Queries.Gas.Consumptions;

public sealed class GetAllGasConsumptionsSpec : BaseSpecification<GasConsumptionEntity>
{
    public GetAllGasConsumptionsSpec(GetGasConsumptionsQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        PopulateFilters(filters);
        ApplyOrdering(filters);

        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetGasConsumptionsQueryFilters filters)
    {
        Query.Where(gc => gc.Status == GasConsumptionStatus.Active && gc.CorrectionForId == null);

        if (filters.FarmIds is not null && filters.FarmIds.Any())
        {
            Query.Where(gc => filters.FarmIds.Contains(gc.FarmId));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<GasConsumptionEntity>();
            
            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
        }
    }

    private void ApplyOrdering(GetGasConsumptionsQueryFilters filters)
    {
        var isDescending = filters.IsDescending;

        switch (filters.OrderBy)
        {
            case GasConsumptionsOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(gc => gc.Farm.Name);
                }
                else
                {
                    Query.OrderBy(gc => gc.Farm.Name);
                }

                break;

            case GasConsumptionsOrderBy.QuantityConsumed:
                if (isDescending)
                {
                    Query.OrderByDescending(gc => gc.QuantityConsumed);
                }
                else
                {
                    Query.OrderBy(gc => gc.QuantityConsumed);
                }

                break;

            case GasConsumptionsOrderBy.Cost:
                if (isDescending)
                {
                    Query.OrderByDescending(gc => gc.Cost);
                }
                else
                {
                    Query.OrderBy(gc => gc.Cost);
                }

                break;

            case GasConsumptionsOrderBy.Cycle:
            default:
                if (isDescending)
                {
                    Query.OrderByDescending(gc => gc.Cycle.Identifier)
                        .ThenByDescending(gc => gc.Cycle.Year);
                }
                else
                {
                    Query.OrderBy(gc => gc.Cycle.Year)
                        .ThenBy(gc => gc.Cycle.Identifier);
                }

                break;
        }
    }
}