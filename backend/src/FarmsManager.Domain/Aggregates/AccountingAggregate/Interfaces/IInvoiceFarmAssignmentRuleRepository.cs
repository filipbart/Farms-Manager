using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;

public interface IInvoiceFarmAssignmentRuleRepository : IRepository<InvoiceFarmAssignmentRuleEntity>
{
    Task<List<InvoiceFarmAssignmentRuleEntity>> GetAllActiveOrderedByPriorityAsync(CancellationToken cancellationToken = default);
    Task<int> GetNextPriorityAsync(CancellationToken cancellationToken = default);
}
