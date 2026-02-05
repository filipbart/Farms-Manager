using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;

public interface IInvoiceUserAssignmentRuleRepository : IRepository<InvoiceUserAssignmentRuleEntity>
{
    Task<List<InvoiceUserAssignmentRuleEntity>> GetAllActiveOrderedByPriorityAsync(CancellationToken cancellationToken = default);
    Task<int> GetNextPriorityAsync(CancellationToken cancellationToken = default);
}
