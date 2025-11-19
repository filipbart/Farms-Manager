using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Farms;

public sealed class FarmByNameSpec : BaseSpecification<FarmEntity>, ISingleResultSpecification<FarmEntity>
{
    public FarmByNameSpec(string name)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => EF.Functions.ILike(t.Name, $"%{name}%"));
    }
}
