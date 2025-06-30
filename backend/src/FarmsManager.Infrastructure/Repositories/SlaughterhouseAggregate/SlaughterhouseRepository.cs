using AutoMapper;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.SlaughterhouseAggregate;

public class SlaughterhouseRepository : AbstractRepository<SlaughterhouseEntity>, ISlaughterhouseRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public SlaughterhouseRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context, configurationProvider)
    {
        _context = context;
    }
}