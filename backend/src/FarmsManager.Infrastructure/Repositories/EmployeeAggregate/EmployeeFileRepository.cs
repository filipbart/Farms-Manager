using AutoMapper;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.EmployeeAggregate;

public class EmployeeFileRepository : AbstractRepository<EmployeeFileEntity>, IEmployeeFileRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public EmployeeFileRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context, configurationProvider)
    {
        _context = context;
    }
}