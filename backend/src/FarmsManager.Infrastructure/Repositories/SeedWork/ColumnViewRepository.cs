using AutoMapper;
using FarmsManager.Domain.Aggregates.SeedWork.Entities;
using FarmsManager.Domain.Aggregates.SeedWork.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.SeedWork;

public class ColumnViewRepository : AbstractRepository<ColumnViewEntity>, IColumnViewRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public ColumnViewRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context, configurationProvider)
    {
        _context = context;
    }
}