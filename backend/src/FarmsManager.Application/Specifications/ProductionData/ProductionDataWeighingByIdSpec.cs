using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

namespace FarmsManager.Application.Specifications.ProductionData;

public sealed class ProductionDataWeighingByIdSpec : BaseSpecification<ProductionDataWeighingEntity>,
    ISingleResultSpecification<ProductionDataWeighingEntity>
{
    public ProductionDataWeighingByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}