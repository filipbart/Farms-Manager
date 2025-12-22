using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class InvoiceAssignmentRuleRepository : AbstractRepository<InvoiceAssignmentRuleEntity>,
    IInvoiceAssignmentRuleRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public InvoiceAssignmentRuleRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) :
        base(context, configurationProvider)
    {
        _context = context;
    }

    public async Task<List<InvoiceAssignmentRuleEntity>> GetAllActiveOrderedByPriorityAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<InvoiceAssignmentRuleEntity>()
            .Where(r => r.IsActive && r.DateDeletedUtc == null)
            .OrderBy(r => r.Priority)
            .Include(r => r.AssignedUser)
            .Include(r => r.TaxBusinessEntity)
            .Include(r => r.Farm)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetNextPriorityAsync(CancellationToken cancellationToken = default)
    {
        var maxPriority = await _context.Set<InvoiceAssignmentRuleEntity>()
            .Where(r => r.DateDeletedUtc == null)
            .MaxAsync(r => (int?)r.Priority, cancellationToken);

        return (maxPriority ?? 0) + 1;
    }
}
