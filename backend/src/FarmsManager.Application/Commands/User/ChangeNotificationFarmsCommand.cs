using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.User;

public record ChangeNotificationFarmsCommand(List<Guid> Farms) : IRequest<EmptyBaseResponse>;

public class ChangeNotificationFarmsCommandHandler : IRequestHandler<ChangeNotificationFarmsCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public ChangeNotificationFarmsCommandHandler(IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<EmptyBaseResponse> Handle(ChangeNotificationFarmsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), cancellationToken) ??
                   throw DomainException.UserNotFound();

        user.ChangeNotificationFarms(request.Farms);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new EmptyBaseResponse();
    }
}