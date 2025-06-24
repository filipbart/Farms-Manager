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
        switch (filters.OrderBy)
        {
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