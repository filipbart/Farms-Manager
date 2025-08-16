using Ardalis.GuardClauses;
using Ardalis.Specification;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Auth;

public record CheckPermissionQuery(string Permission) : IRequest<bool>;

public class CheckPermissionQueryHandler : IRequestHandler<CheckPermissionQuery, bool>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public CheckPermissionQueryHandler(IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(CheckPermissionQuery request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId();
        Guard.Against.Null(userId, nameof(userId));

        var spec = new UserWithPermissionsSpec(userId.Value);
        var user = await _userRepository.FirstOrDefaultAsync(spec, cancellationToken);
        Guard.Against.Null(user, nameof(user));

        return user.GetPermissions().Contains(request.Permission) || user.IsAdmin;
    }
}

public sealed class UserWithPermissionsSpec : BaseSpecification<UserEntity>, ISingleResultSpecification<UserEntity>
{
    public UserWithPermissionsSpec(Guid userId)
    {
        EnsureExists();
        Query.Where(t => t.Id == userId);
        Query.Include(t => t.Permissions);
    }
}