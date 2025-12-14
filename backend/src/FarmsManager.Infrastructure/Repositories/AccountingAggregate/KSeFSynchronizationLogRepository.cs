using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

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
}