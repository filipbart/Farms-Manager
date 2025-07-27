using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Henhouses;

public sealed class HenhouseByNameSpec : BaseSpecification<HenhouseEntity>, ISingleResultSpecification<HenhouseEntity>
{
    public HenhouseByNameSpec(string name)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => EF.Functions.ILike(t.Name, $"%{name}%"));
    }
}