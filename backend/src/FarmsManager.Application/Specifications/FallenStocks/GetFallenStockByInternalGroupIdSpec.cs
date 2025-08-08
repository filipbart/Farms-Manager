using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;

namespace FarmsManager.Application.Specifications.FallenStocks;

public sealed class GetFallenStockByInternalGroupIdSpec : BaseSpecification<FallenStockEntity>
{
    public GetFallenStockByInternalGroupIdSpec(Guid internalGroupId)
    {
        EnsureExists();
        Query.Where(t => t.InternalGroupId == internalGroupId);
    }
}