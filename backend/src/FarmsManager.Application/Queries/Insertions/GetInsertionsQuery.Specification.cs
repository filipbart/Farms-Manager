using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Queries.Insertions;

public sealed class GetAllInsertionsSpec : BaseSpecification<InsertionEntity>
{
    public GetAllInsertionsSpec(GetInsertionsQueryFilters filters, bool withPagination)
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

    private void PopulateFilters(GetInsertionsQueryFilters filters)
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
            var predicate = PredicateBuilder.New<InsertionEntity>();
            
            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
        }

        if (filters.DateSince is not null)
        {
            Query.Where(t => t.InsertionDate >= filters.DateSince);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(t => t.InsertionDate <= filters.DateTo);
        }
    }

    private void ApplyOrdering(GetInsertionsQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case InsertionOrderBy.Cycle:
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

            case InsertionOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Farm.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Farm.Name);
                }

                break;

            case InsertionOrderBy.Henhouse:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Henhouse.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Henhouse.Name);
                }

                break;

            case InsertionOrderBy.InsertionDate:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.InsertionDate);
                }
                else
                {
                    Query.OrderBy(t => t.InsertionDate);
                }

                break;

            case InsertionOrderBy.Quantity:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Quantity);
                }
                else
                {
                    Query.OrderBy(t => t.Quantity);
                }

                break;

            case InsertionOrderBy.Hatchery:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Hatchery.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Hatchery.Name);
                }

                break;

            case InsertionOrderBy.BodyWeight:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.BodyWeight);
                }
                else
                {
                    Query.OrderBy(t => t.BodyWeight);
                }

                break;
            case InsertionOrderBy.DateCreatedUtc:
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