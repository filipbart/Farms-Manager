using System.Security.Claims;
using FarmsManager.Application.Common.Constants;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;

namespace FarmsManager.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.Claims.Single(t => t.Type == ClaimNames.UserId).Value;
        if (userId.IsEmpty())
            throw DomainException.Unauthorized();

        return Guid.Parse(userId);
    }
}