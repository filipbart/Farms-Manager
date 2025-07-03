using AutoMapper;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.FarmAggregate;

public class SaleRepository : AbstractRepository<SaleEntity>, ISaleRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public SaleRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context, configurationProvider)
    {
        _context = context;
    }
}