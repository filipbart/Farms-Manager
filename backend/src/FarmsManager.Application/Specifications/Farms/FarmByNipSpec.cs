using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Specifications.Farms;

public sealed class FarmByNipSpec : BaseSpecification<FarmEntity>, ISingleResultSpecification<FarmEntity>
{
    public FarmByNipSpec(string nip)
    {
        nip = nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.Nip == nip);
    }
}
