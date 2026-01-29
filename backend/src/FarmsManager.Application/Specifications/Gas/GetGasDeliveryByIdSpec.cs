using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;

namespace FarmsManager.Application.Specifications.Gas;

public class GetGasDeliveryByIdSpec : BaseSpecification<GasDeliveryEntity>,
    ISingleResultSpecification<GasDeliveryEntity>
{
    public GetGasDeliveryByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
        Query.Include(t => t.Farm);
    }
}