using AutoMapper;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.GasAggregate;

public class GasConsumptionSourceRepository : AbstractRepository<GasConsumptionSourceEntity>, IGasConsumptionSourceRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public GasConsumptionSourceRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) :
        base(context,
            configurationProvider)
    {
        _context = context;
    }
}