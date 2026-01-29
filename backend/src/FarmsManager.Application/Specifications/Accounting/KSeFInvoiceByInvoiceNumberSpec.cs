using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.Accounting;

public sealed class KSeFInvoiceByInvoiceNumberSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public KSeFInvoiceByInvoiceNumberSpec(string invoiceNumber)
    {
        EnsureExists();
        Query.Where(i => i.InvoiceNumber == invoiceNumber);
    }
}
