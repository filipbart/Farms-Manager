using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Specifications.Sales;

public sealed class SaleByIdSpec : BaseSpecification<SaleEntity>, ISingleResultSpecification<SaleEntity>
{
    public SaleByIdSpec(Guid saleId)
    {
        EnsureExists();
        Query.Where(t => t.Id == saleId);
    }
}