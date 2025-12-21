using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;

public sealed class GetKSeFInvoiceByIdSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public GetKSeFInvoiceByIdSpec(Guid invoiceId)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(x => x.Id == invoiceId);
        Query.Include(x => x.AssignedCycle);
        Query.Include(x => x.AssignedUser);
        Query.Include(x => x.Farm);
        Query.Include(x => x.Creator);
    }
}
