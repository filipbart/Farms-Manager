using Ardalis.GuardClauses;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Commands.Auth;

public class RefreshTokenCommand : IRequest<BaseResponse<RefreshTokenCommandResponse>>;

public record RefreshTokenCommandResponse(string AccessToken);

public class
    RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, BaseResponse<RefreshTokenCommandResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserSessionService _userSessionService;

    public RefreshTokenCommandHandler(
        IUserSessionService userSessionService,
        IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _userSessionService = userSessionService;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<RefreshTokenCommandResponse>> Handle(RefreshTokenCommand request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId();
        var user = await _userRepository.GetByIdAsync(userId, ct);
        Guard.Against.Null(user);

        await _userSessionService.DeactivateSessionAsync(ct);
        var tokenResponse = await _userSessionService.GenerateToken(user, ct);
        return BaseResponse.CreateResponse(new RefreshTokenCommandResponse(tokenResponse.Token));
    }
}