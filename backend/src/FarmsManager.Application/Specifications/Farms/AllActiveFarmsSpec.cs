using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Specifications.Farms;

public sealed class AllActiveFarmsSpec : BaseSpecification<FarmEntity>
{
    public AllActiveFarmsSpec()
    {
        DisableTracking();
        EnsureExists();
        Query.Include(f => f.ActiveCycle);
    }
}
