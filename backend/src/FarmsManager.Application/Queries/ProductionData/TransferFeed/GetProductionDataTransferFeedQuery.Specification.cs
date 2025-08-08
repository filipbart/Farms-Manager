using Ardalis.Specification;
using FarmsManager.Application.Models.ProductionData;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Queries.ProductionData.TransferFeed;

public sealed class GetAllProductionDataTransferFeedSpec : BaseSpecification<ProductionDataTransferFeedEntity>
{
    public GetAllProductionDataTransferFeedSpec(ProductionDataQueryFilters filters, bool withPagination)
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
            Query.Where(t => filters.FarmIds.Contains(t.FromFarmId) || filters.FarmIds.Contains(t.ToFarmId));
        }

        if (filters.HenhouseIds is not null && filters.HenhouseIds.Any())
        {
            Query.Where(t =>
                filters.HenhouseIds.Contains(t.FromHenhouseId) || filters.HenhouseIds.Contains(t.ToHenhouseId));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<ProductionDataTransferFeedEntity>();

            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    (t.FromCycle.Identifier == cycleFilter.Identifier && t.FromCycle.Year == cycleFilter.Year) ||
                    (t.ToCycle.Identifier == cycleFilter.Identifier && t.ToCycle.Year == cycleFilter.Year)
                ));

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

    private void ApplyOrdering(ProductionDataQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case ProductionDataOrderBy.Cycle:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.FromCycle.Identifier)
                        .ThenByDescending(t => t.FromCycle.Year);
                }
                else
                {
                    Query.OrderBy(t => t.FromCycle.Year)
                        .ThenBy(t => t.FromCycle.Identifier);
                }

                break;

            case ProductionDataOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.FromFarm.Name);
                }
                else
                {
                    Query.OrderBy(t => t.FromFarm.Name);
                }

                break;

            case ProductionDataOrderBy.Henhouse:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.FromHenhouse.Name);
                }
                else
                {
                    Query.OrderBy(t => t.FromHenhouse.Name);
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