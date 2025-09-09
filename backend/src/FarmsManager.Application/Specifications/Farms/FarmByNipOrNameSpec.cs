using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Farms;

public sealed class FarmByNipOrNameSpec : BaseSpecification<FarmEntity>, ISingleResultSpecification<FarmEntity>
{
    public FarmByNipOrNameSpec(string nip, string name)
    {
        nip = nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
        EnsureExists();
        DisableTracking();

        Query.Where(t => EF.Functions.ILike(t.Name, $"%{name}%") || t.Nip == nip);
    }
}