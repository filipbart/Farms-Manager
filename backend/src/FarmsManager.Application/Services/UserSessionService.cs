using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.UserSessions;
using FarmsManager.Domain.Aggregates.UserAggregate;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;

namespace FarmsManager.Application.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IUserAccessTokenGenerator _accessTokenGenerator;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IUserDataResolver _userDataResolver;

    public UserSessionService(IUserAccessTokenGenerator accessTokenGenerator,
        IUserSessionRepository userSessionRepository, IUserDataResolver userDataResolver)
    {
        _accessTokenGenerator = accessTokenGenerator;
        _userSessionRepository = userSessionRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<GenerateTokenResponse> GenerateToken(UserEntity user, CancellationToken ct)
    {
        var tokenResponse = _accessTokenGenerator.GenerateToken(user);

        var userSession = UserSessionEntity.Create(user.Id, tokenResponse.SessionId);
        await _userSessionRepository.AddAsync(userSession, ct);

        return tokenResponse;
    }

    public async Task DeactivateSessionAsync(CancellationToken ct)
    {
        var sessionId = _userDataResolver.TryGetSessionId();
        if (sessionId is not null)
        {
            var userSession =
                await _userSessionRepository.SingleOrDefaultAsync(new UserSessionBySessionIdSpec(sessionId.Value), ct);
            userSession?.Deactivate();
        }
    }
}