using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using ILogger = Serilog.ILogger;

namespace FarmsManager.Api.Middleware;

public class DomainExceptionMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public DomainExceptionMiddleware(ILogger logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            _logger.Error(ex.GetInnerException() ?? ex,
                "Error Name: {ErrorName}\nError Description: {ErrorDescription}", ex.Name, ex.Description);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)ex.StatusCode;
            var details = new ExceptionDetailsDto
            {
                ErrorDescription = ex.Description,
                ErrorName = ex.Name
            };
            await context.Response.WriteAsync(details.ToJsonString());
        }
        catch (Exception ex)
        {
            var details = new ExceptionDetailsDto
            {
                ErrorDescription = "Internal server error",
                ErrorName = "InternalServerError"
            };

            _logger.Error(ex,
                "Error Name: {ErrorName}\nError Description: {ErrorDescription}", details.ErrorName,
                details.ErrorDescription);

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