using AutoMapper;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.ProductionDataAggregate;

public class ProductionDataFlockLossMeasureRepository : AbstractRepository<ProductionDataFlockLossMeasureEntity>,
    IProductionDataFlockLossMeasureRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public ProductionDataFlockLossMeasureRepository(FarmsManagerContext context,
        IConfigurationProvider configurationProvider) : base(context, configurationProvider)
    {
        _context = context;
    }
}