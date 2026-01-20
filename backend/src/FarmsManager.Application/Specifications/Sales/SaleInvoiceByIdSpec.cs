using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;

namespace FarmsManager.Application.Specifications.Sales;

public class SaleInvoiceByIdSpec : BaseSpecification<SaleInvoiceEntity>, ISingleResultSpecification<SaleInvoiceEntity>
{
    public SaleInvoiceByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
        Query.Include(t => t.Creator);
        Query.Include(t => t.Modifier);
        Query.Include(t => t.Deleter);
        Query.Include(t => t.Farm);
    }
}