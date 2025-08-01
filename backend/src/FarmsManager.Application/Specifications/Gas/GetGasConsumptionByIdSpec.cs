using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;

namespace FarmsManager.Application.Specifications.Gas;

public class GetGasConsumptionByIdSpec : BaseSpecification<GasConsumptionEntity>,
    ISingleResultSpecification<GasConsumptionEntity>
{
    public GetGasConsumptionByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);

        Query.Include(t => t.ConsumptionSources);
    }
}