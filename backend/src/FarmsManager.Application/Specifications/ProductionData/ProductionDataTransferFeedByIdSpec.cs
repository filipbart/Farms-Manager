using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

namespace FarmsManager.Application.Specifications.ProductionData;

public sealed class ProductionDataTransferFeedByIdSpec : BaseSpecification<ProductionDataTransferFeedEntity>,
    ISingleResultSpecification<ProductionDataTransferFeedEntity>
{
    public ProductionDataTransferFeedByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}