using FarmsManager.Domain.Aggregates.GasAggregate.Entities;

namespace FarmsManager.Application.Specifications.Gas;

public sealed class GetAllGasContractorsSpec : BaseSpecification<GasContractorEntity>
{
    public GetAllGasContractorsSpec()
    {
        EnsureExists();
    }
}