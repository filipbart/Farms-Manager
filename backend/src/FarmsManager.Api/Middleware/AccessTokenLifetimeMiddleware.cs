using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.UserSessions;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;

namespace FarmsManager.Api.Middleware;

public class AccessTokenLifetimeMiddleware : IMiddleware
{
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IConfiguration _configuration;

    public AccessTokenLifetimeMiddleware(IUserSessionRepository userSessionRepository,
        IUserDataResolver userDataResolver, IConfiguration configuration)
    {
        _userSessionRepository = userSessionRepository;
        _userDataResolver = userDataResolver;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
        if (attribute is not null)
        {
            await next(context);
            return;
        }

        var sessionId = _userDataResolver.TryGetSessionId();
        if (sessionId is not null)
        {
            var userSession =
                await _userSessionRepository.SingleOrDefaultAsync(new UserSessionBySessionIdSpec(sessionId.Value));
            var sessionLength = _configuration.GetValue<int>("SessionLength");
            if (userSession?.IsValid(sessionLength) == true)
            {
                userSession.UpdateLastSeenAt();
                await _userSessionRepository.UpdateAsync(userSession);
                await next(context);
                return;
            }
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }
}