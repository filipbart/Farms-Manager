using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class InvoiceFarmAssignmentRuleRepository : AbstractRepository<InvoiceFarmAssignmentRuleEntity>,
    IInvoiceFarmAssignmentRuleRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public InvoiceFarmAssignmentRuleRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) :
        base(context, configurationProvider)
    {
        _context = context;
    }

    public async Task<List<InvoiceFarmAssignmentRuleEntity>> GetAllActiveOrderedByPriorityAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<InvoiceFarmAssignmentRuleEntity>()
            .Where(r => r.IsActive && r.DateDeletedUtc == null)
            .OrderBy(r => r.Priority)
            .Include(r => r.TaxBusinessEntity)
            .Include(r => r.TargetFarm)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetNextPriorityAsync(CancellationToken cancellationToken = default)
    {
        var maxPriority = await _context.Set<InvoiceFarmAssignmentRuleEntity>()
            .Where(r => r.DateDeletedUtc == null)
            .MaxAsync(r => (int?)r.Priority, cancellationToken);

        return (maxPriority ?? 0) + 1;
    }
}
