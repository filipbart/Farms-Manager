using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;

namespace FarmsManager.Application.Specifications.Sales;

public sealed class SlaughterhouseByNameSpec : BaseSpecification<SlaughterhouseEntity>,
    ISingleResultSpecification<SlaughterhouseEntity>
{
    public SlaughterhouseByNameSpec(string name)
    {
        DisableTracking();

        Query.Where(t => t.Name.ToLower() == name.ToLower());
    }
}
