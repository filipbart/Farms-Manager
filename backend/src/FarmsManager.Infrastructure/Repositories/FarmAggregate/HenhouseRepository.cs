using AutoMapper;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.FarmAggregate;

public class HenhouseRepository : AbstractRepository<HenhouseEntity>, IHenhouseRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public HenhouseRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(context,
        configurationProvider)
    {
        _context = context;
    }
}