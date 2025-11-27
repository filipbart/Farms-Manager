using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class KSeFSynchronizationLogRepository : AbstractRepository<KSeFSynchronizationLogEntity>,
    IKSeFSynchronizationLogRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public KSeFSynchronizationLogRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) :
        base(
            context,
            configurationProvider)
    {
        _context = context;
    }

    public async Task<KSeFSynchronizationLogEntity> GetLastSynchronizationAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<KSeFSynchronizationLogEntity>()
            .Where(x => x.DateDeletedUtc.HasValue == false)
            .OrderByDescending(x => x.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<KSeFSynchronizationLogEntity>> GetSynchronizationHistoryAsync(int count,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<KSeFSynchronizationLogEntity>()
            .Where(x => x.DateDeletedUtc.HasValue == false)
            .OrderByDescending(x => x.StartedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}