using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Farms;

public class FarmsByNipOrNameSpec : BaseSpecification<FarmEntity>
{
    public FarmsByNipOrNameSpec(string nip, string name)
    {
        nip = nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
        EnsureExists();
        DisableTracking();

        Query.Where(t => EF.Functions.ILike(t.Name, $"%{name}%") || t.Nip == nip);
    }
}