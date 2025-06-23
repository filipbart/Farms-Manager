using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Specifications.Farms;

public class FarmByIdSpec : BaseSpecification<FarmEntity>, ISingleResultSpecification<FarmEntity>
{
    public FarmByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
        Query.Include(t => t.ActiveCycle);
    }
}