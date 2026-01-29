using FarmsManager.Api.Attributes;
using FarmsManager.Application.Queries.Auth;
using FarmsManager.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;

namespace FarmsManager.Api.Middleware;

public class CheckPermissionMiddleware(IMediator mediator) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var allowAnonymousAttribute = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
        var hasPermissionAttributes = endpoint?.Metadata.GetOrderedMetadata<HasPermissionAttribute>() ?? [];

        if (allowAnonymousAttribute is null && hasPermissionAttributes.Count > 0)
        {
            // Sprawdź wszystkie wymagane uprawnienia (zarówno z kontrolera jak i metody)
            foreach (var permissionAttribute in hasPermissionAttributes)
            {
                var allow = await mediator.Send(new CheckPermissionQuery(permissionAttribute.Permission));
                if (!allow)
                {
                    throw DomainException.Forbidden();
                }
            }
        }

        await next(context);
    }
}