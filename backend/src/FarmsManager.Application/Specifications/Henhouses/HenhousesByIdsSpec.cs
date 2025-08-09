using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Specifications.Henhouses;

public sealed class HenhousesByIdsSpec : BaseSpecification<HenhouseEntity>
{
    public HenhousesByIdsSpec(Guid[] ids)
    {
        EnsureExists();
        Query.Where(t => ids.Contains(t.Id));
    }
}