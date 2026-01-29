using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;

public interface IInvoiceModuleAssignmentRuleRepository : IRepository<InvoiceModuleAssignmentRuleEntity>
{
    Task<List<InvoiceModuleAssignmentRuleEntity>> GetAllActiveOrderedByPriorityAsync(CancellationToken cancellationToken = default);
    Task<List<InvoiceModuleAssignmentRuleEntity>> GetAllOrderedByPriorityAsync(CancellationToken cancellationToken = default);
    Task<int> GetNextPriorityAsync(CancellationToken cancellationToken = default);
}
