using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Specifications.Cycle;

public sealed class GetAllCyclesSpec : BaseSpecification<CycleEntity>
{
    public GetAllCyclesSpec()
    {
        EnsureExists();
        Query.OrderBy(t => t.Year).ThenBy(t => t.Identifier);
    }
}