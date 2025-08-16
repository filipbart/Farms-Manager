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
        var hasPermissionAttribute = endpoint?.Metadata.GetMetadata<HasPermissionAttribute>();

        if (allowAnonymousAttribute is null && hasPermissionAttribute != null)
        {
            var allow = await mediator.Send(new CheckPermissionQuery(hasPermissionAttribute.Permission));
            if (!allow)
            {
                throw DomainException.Forbidden();
            }
        }

        await next(context);
    }
}