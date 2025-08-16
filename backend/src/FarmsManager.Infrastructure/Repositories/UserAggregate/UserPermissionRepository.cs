using AutoMapper;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.UserAggregate;

public class UserPermissionRepository : AbstractRepository<UserPermissionEntity>, IUserPermissionRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public UserPermissionRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context,
        configurationProvider)
    {
        _context = context;
    }
}