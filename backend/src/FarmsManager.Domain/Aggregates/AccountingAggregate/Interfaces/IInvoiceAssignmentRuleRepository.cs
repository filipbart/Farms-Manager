using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;

public interface IInvoiceAssignmentRuleRepository : IRepository<InvoiceAssignmentRuleEntity>
{
    Task<List<InvoiceAssignmentRuleEntity>> GetAllActiveOrderedByPriorityAsync(CancellationToken cancellationToken = default);
    Task<int> GetNextPriorityAsync(CancellationToken cancellationToken = default);
}
