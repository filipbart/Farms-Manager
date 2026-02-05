using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class InvoiceUserAssignmentRuleRepository : AbstractRepository<InvoiceUserAssignmentRuleEntity>,
    IInvoiceUserAssignmentRuleRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public InvoiceUserAssignmentRuleRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) :
        base(context, configurationProvider)
    {
        _context = context;
    }

    public async Task<List<InvoiceUserAssignmentRuleEntity>> GetAllActiveOrderedByPriorityAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<InvoiceUserAssignmentRuleEntity>()
            .Where(r => r.IsActive && r.DateDeletedUtc == null)
            .OrderBy(r => r.Priority)
            .Include(r => r.AssignedUser)
            .Include(r => r.TaxBusinessEntity)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetNextPriorityAsync(CancellationToken cancellationToken = default)
    {
        var maxPriority = await _context.Set<InvoiceUserAssignmentRuleEntity>()
            .Where(r => r.DateDeletedUtc == null)
            .MaxAsync(r => (int?)r.Priority, cancellationToken);

        return (maxPriority ?? 0) + 1;
    }
}
