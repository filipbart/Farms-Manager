using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

namespace FarmsManager.Application.Specifications.ProductionData.Failures;

public sealed class ProductionDataFailureByIdSpec : BaseSpecification<ProductionDataFailureEntity>,
    ISingleResultSpecification<ProductionDataFailureEntity>
{
    public ProductionDataFailureByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}