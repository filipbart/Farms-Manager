using AutoMapper;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.HatcheryAggregate;

public class HatcheryRepository : AbstractRepository<HatcheryEntity>, IHatcheryRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public HatcheryRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(context,
        configurationProvider)
    {
        _context = context;
    }
}