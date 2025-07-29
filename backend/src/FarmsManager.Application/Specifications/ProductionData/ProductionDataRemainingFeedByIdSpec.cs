using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

namespace FarmsManager.Application.Specifications.ProductionData;

public sealed class ProductionDataRemainingFeedByIdSpec : BaseSpecification<ProductionDataRemainingFeedEntity>,
    ISingleResultSpecification<ProductionDataRemainingFeedEntity>
{
    public ProductionDataRemainingFeedByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}