using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;

namespace FarmsManager.Application.Specifications.Gas;

public sealed class GasContractorByNameSpec : BaseSpecification<GasContractorEntity>,
    ISingleResultSpecification<GasContractorEntity>
{
    public GasContractorByNameSpec(string name)
    {
        DisableTracking();

        Query.Where(t => t.Name.ToLower() == name.ToLower());
    }
}
