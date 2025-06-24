using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Queries.Insertions;

public sealed class GetAllInsertionsSpec : BaseSpecification<InsertionEntity>
{
    public GetAllInsertionsSpec(GetInsertionsQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        PopulateFilters(filters);
        if (withPagination)
        {
            Paginate(filters);
            ApplyOrdering(filters);
        }
    }

    private void PopulateFilters(GetInsertionsQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Any())
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.HenhouseIds is not null && filters.HenhouseIds.Any())
        {
            Query.Where(t => filters.HenhouseIds.Contains(t.HenhouseId));
        }

        if (filters.HatcheryIds is not null && filters.HatcheryIds.Any())
        {
            Query.Where(t => filters.HatcheryIds.Contains(t.HatcheryId));
        }

        if (filters.Cycles is not null && filters.Cycles.Any())
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
                        .ThenByDescending(t => t.Cycle.Year);
                }
                else
                {
                    Query.OrderBy(t => t.Cycle.Identifier)
                        .ThenBy(t => t.Cycle.Year);
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
                if (filters.IsDescending)
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