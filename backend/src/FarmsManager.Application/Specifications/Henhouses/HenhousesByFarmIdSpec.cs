using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Specifications.Henhouses;

public sealed class HenhousesByFarmIdSpec : BaseSpecification<HenhouseEntity>
{
    public HenhousesByFarmIdSpec(Guid farmId)
    {
        EnsureExists();

        Query.Where(t => t.FarmId == farmId);
        Query.OrderBy(t => t.Name);
    }
}