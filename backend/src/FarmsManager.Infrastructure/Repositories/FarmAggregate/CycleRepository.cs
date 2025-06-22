using AutoMapper;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.FarmAggregate;

public class CycleRepository : AbstractRepository<CycleEntity>, ICycleRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public CycleRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(context,
        configurationProvider)
    {
        _context = context;
    }
}