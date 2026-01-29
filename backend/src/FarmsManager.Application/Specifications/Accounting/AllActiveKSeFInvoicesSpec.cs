using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.Accounting;

public sealed class AllActiveKSeFInvoicesSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public AllActiveKSeFInvoicesSpec()
    {
        EnsureExists();
    }
}
