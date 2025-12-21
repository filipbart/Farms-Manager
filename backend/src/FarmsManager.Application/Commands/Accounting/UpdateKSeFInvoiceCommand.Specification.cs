using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Commands.Accounting;

public sealed class KSeFInvoiceByIdSpec : BaseSpecification<KSeFInvoiceEntity>, ISingleResultSpecification<KSeFInvoiceEntity>
{
    public KSeFInvoiceByIdSpec(Guid invoiceId)
    {
        EnsureExists();
        Query.Where(x => x.Id == invoiceId);
    }
}
