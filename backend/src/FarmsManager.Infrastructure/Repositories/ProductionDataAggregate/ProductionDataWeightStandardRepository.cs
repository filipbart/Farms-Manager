using AutoMapper;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.ProductionDataAggregate;

public class ProductionDataWeightStandardRepository : AbstractRepository<ProductionDataWeightStandardEntity>,
    IProductionDataWeightStandardRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public ProductionDataWeightStandardRepository(FarmsManagerContext context,
        IConfigurationProvider configurationProvider) : base(context, configurationProvider)
    {
        _context = context;
    }
}