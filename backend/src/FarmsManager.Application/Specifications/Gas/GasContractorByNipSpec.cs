using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;

namespace FarmsManager.Application.Specifications.Gas;

public sealed class GasContractorByNipSpec : BaseSpecification<GasContractorEntity>,
    ISingleResultSpecification<GasContractorEntity>
{
    public GasContractorByNipSpec(string nip)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.Nip == nip);
    }
}