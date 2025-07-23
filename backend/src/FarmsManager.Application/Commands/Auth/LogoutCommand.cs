using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Auth;

public record LogoutCommand : IRequest<EmptyBaseResponse>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, EmptyBaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserSessionService _userSessionService;

    public LogoutCommandHandler(IUserRepository userRepository, IUserDataResolver userDataResolver,
        IUserSessionService userSessionService)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _userSessionService = userSessionService;
    }

    public async Task<EmptyBaseResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            throw DomainException.Unauthorized();

        await _userRepository.SaveChangesAsync(cancellationToken);

        await _userSessionService.DeactivateSessionAsync(cancellationToken);

        return new EmptyBaseResponse();
    }
}