using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Auth;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Users;

public record UpdateUserPermissionsDto
{
    public List<string> Permissions { get; init; } = [];
}

public record UpdateUserPermissionsCommand(Guid UserId, List<string> Permissions) : IRequest<EmptyBaseResponse>;

public class UpdateUserPermissionsCommandHandler : IRequestHandler<UpdateUserPermissionsCommand, EmptyBaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserPermissionRepository _userPermissionRepository;

    public UpdateUserPermissionsCommandHandler(IUserRepository userRepository, IUserDataResolver userDataResolver,
        IUserPermissionRepository userPermissionRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _userPermissionRepository = userPermissionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateUserPermissionsCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserWithPermissionsSpec(request.UserId), ct);

        await _userPermissionRepository.DeleteRangeAsync(user.Permissions, ct);

        foreach (var requestPermission in request.Permissions)
        {
            if (user.Permissions.Any(t => t.PermissionName == requestPermission))
            {
                user.RemovePermission(requestPermission);
            }
            else
            {
                user.AddPermission(requestPermission, userId);
            }
        }

        await _userRepository.UpdateAsync(user, ct);
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateUserPermissionsCommandValidator : AbstractValidator<UpdateUserPermissionsCommand>
{
    public UpdateUserPermissionsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Permissions).NotNull();
    }
}