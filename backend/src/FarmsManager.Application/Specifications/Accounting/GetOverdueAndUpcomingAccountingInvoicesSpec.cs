using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.Accounting;

public sealed class GetOverdueAndUpcomingAccountingInvoicesSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public GetOverdueAndUpcomingAccountingInvoicesSpec(DateOnly date, Guid? userId, List<Guid> accessibleFarmIds)
    {
        EnsureExists();

        // Filter by accessible farms if provided
        // If user has farm restrictions, only show invoices for those farms (exclude invoices without farm assignment)
        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => p.FarmId.HasValue && accessibleFarmIds.Contains(p.FarmId.Value));

        // Only unpaid invoices
        Query.Where(t => t.PaymentDate.HasValue == false);

        // Only invoices with due date
        Query.Where(t => t.PaymentDueDate.HasValue);

        // Due date within range (up to 14 days from now)
        Query.Where(t => t.PaymentDueDate.Value <= date);

        // Filter by assigned user: show if assigned to current user OR not assigned to anyone
        if (userId.HasValue)
        {
            Query.Where(t => t.AssignedUserId == null || t.AssignedUserId == userId.Value);
        }
    }
}
