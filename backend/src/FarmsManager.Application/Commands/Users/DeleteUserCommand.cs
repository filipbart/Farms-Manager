using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Users;

public record DeleteUserCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var entity = await _userRepository.GetAsync(new UserByIdSpec(request.Id), cancellationToken);

        entity.Delete(userId);
        await _userRepository.UpdateAsync(entity, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}