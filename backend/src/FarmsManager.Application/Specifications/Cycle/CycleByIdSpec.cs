using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Specifications.Cycle;

public sealed class CycleByIdSpec : BaseSpecification<CycleEntity>, ISingleResultSpecification<CycleEntity>
{
    public CycleByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}