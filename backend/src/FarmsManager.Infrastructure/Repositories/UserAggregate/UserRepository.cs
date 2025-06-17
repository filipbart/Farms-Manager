using AutoMapper;
using FarmsManager.Domain.Aggregates.UserAggregate.Entites;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.UserAggregate;

public class UserRepository : AbstractRepository<UserEntity>, IUserRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public UserRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(context,
        configurationProvider)
    {
        _context = context;
    }
}