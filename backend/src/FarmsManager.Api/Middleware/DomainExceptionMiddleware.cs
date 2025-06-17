using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;

namespace FarmsManager.Api.Middleware;

public class DomainExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)ex.StatusCode;
            var details = new ExceptionDetailsDto
            {
                ErrorDescription = ex.Description,
                ErrorName = ex.Name
            };
            await context.Response.WriteAsync(details.ToJsonString());
        }
        catch (Exception)
        {
            var details = new ExceptionDetailsDto
            {
                ErrorDescription = "Internal server error",
                ErrorName = "InternalServerError"
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(details.ToJsonString());
        }
    }
}

public class ExceptionDetailsDto
{
    public string ErrorName { get; init; }
    public string ErrorDescription { get; init; }
}