using System.IdentityModel.Tokens.Jwt;
using Ardalis.GuardClauses;
using FarmsManager.Application.Common.Constants;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Shared.Extensions;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Services;

public class UserDataResolver : IUserDataResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserDataResolver(IHttpContextAccessor httpContextAccessor)
    {
        Guard.Against.Null(httpContextAccessor);
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetLoginAsync()
    {
        try
        {
            var jwtSecurityToken = GetJwtSecurityToken();
            return jwtSecurityToken.Claims.Single(t => t.Type == ClaimNames.Login).Value;
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetUserId()
    {
        Guard.Against.Null(_httpContextAccessor.HttpContext);
        try
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

            return userId;
        }
        catch
        {
            return null;
        }
    }

    public Guid? TryGetSessionId()
    {
        try
        {
            var jwtSecurityToken = GetJwtSecurityToken();
            var sessionId = jwtSecurityToken.Claims.Single(t => t.Type == ClaimNames.SessionId).Value;
            return Guid.Parse(sessionId);
        }
        catch
        {
            return null;
        }
    }

    private JwtSecurityToken GetJwtSecurityToken()
    {
        var tokenHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"].ToString()
            .Replace("Bearer", string.Empty)
            .Trim();
        if (tokenHeader.IsEmpty())
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                tokenHeader = _httpContextAccessor.HttpContext.Request.Headers["access_token"];
            }
        }

        Guard.Against.NullOrWhiteSpace(tokenHeader);

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(tokenHeader);
        return jwtSecurityToken;
    }
}