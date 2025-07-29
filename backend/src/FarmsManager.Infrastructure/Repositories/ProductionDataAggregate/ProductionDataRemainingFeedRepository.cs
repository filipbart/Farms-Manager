using AutoMapper;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.ProductionDataAggregate;

public class ProductionDataRemainingFeedRepository : AbstractRepository<ProductionDataRemainingFeedEntity>,
    IProductionDataRemainingFeedRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public ProductionDataRemainingFeedRepository(FarmsManagerContext context,
        IConfigurationProvider configurationProvider) : base(context, configurationProvider)
    {
        _context = context;
    }
}