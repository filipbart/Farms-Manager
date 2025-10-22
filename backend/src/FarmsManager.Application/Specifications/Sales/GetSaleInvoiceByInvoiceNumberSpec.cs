using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Sales;

public class GetSaleInvoiceByInvoiceNumberSpec : BaseSpecification<SaleInvoiceEntity>,
    ISingleResultSpecification<SaleInvoiceEntity>
{
    public GetSaleInvoiceByInvoiceNumberSpec(string invoiceNumber)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => EF.Functions.ILike(t.InvoiceNumber, invoiceNumber));
        Query.Include(t => t.Creator);
        Query.Include(t => t.Modifier);
        Query.Include(t => t.Deleter);
    }
}