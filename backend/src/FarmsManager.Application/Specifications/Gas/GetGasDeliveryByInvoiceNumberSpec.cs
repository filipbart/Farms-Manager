using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Gas;

public class GetGasDeliveryByInvoiceNumberSpec : BaseSpecification<GasDeliveryEntity>,
    ISingleResultSpecification<GasDeliveryEntity>
{
    public GetGasDeliveryByInvoiceNumberSpec(string invoiceNumber)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => EF.Functions.ILike(invoiceNumber, t.InvoiceNumber));
    }
}