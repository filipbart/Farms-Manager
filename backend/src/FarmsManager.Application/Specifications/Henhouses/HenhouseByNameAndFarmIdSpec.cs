using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Henhouses;

public sealed class HenhouseByNameAndFarmIdSpec : BaseSpecification<HenhouseEntity>,
    ISingleResultSpecification<HenhouseEntity>
{
    public HenhouseByNameAndFarmIdSpec(string name, Guid? farmId)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.FarmId == farmId);
        Query.Where(t => EF.Functions.ILike(t.Name, $"{name}"));
    }
}