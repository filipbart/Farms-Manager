using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Sales;

public class GetSaleInvoiceByInvoiceNumberAndFarmSpec : BaseSpecification<SaleInvoiceEntity>,
    ISingleResultSpecification<SaleInvoiceEntity>
{
    public GetSaleInvoiceByInvoiceNumberAndFarmSpec(string invoiceNumber, Guid farmId)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => EF.Functions.ILike(t.InvoiceNumber, invoiceNumber)); 
        Query.Where(t => t.FarmId == farmId);
        Query.Include(t => t.Creator);
        Query.Include(t => t.Modifier);
        Query.Include(t => t.Deleter);
    }
}
