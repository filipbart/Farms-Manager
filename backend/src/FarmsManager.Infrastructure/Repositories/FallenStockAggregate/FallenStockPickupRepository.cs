using AutoMapper;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.FallenStockAggregate;

public class FallenStockPickupRepository : AbstractRepository<FallenStockPickupEntity>, IFallenStockPickupRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public FallenStockPickupRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) :
        base(context, configurationProvider)
    {
        _context = context;
    }
}