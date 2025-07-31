using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;

namespace FarmsManager.Application.Specifications.Gas;

public class GetGasContractorByIdSpec : BaseSpecification<GasContractorEntity>,
    ISingleResultSpecification<GasContractorEntity>
{
    public GetGasContractorByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}