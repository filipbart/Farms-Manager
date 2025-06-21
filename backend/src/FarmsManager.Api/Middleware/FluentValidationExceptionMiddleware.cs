using System.Text.Json;
using FarmsManager.Application.Common.Responses;
using FluentValidation;

namespace FarmsManager.Api.Middleware;

public class FluentValidationExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, ValidationException exception)
    {
        var groupedErrors = exception.Errors.GroupBy(x => x.PropertyName);
        var errors = groupedErrors.ToDictionary(t => t.Key, t => string.Join(". ", t));
        var errorResponse = BaseResponse.CreateErrorResponse(errors);
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errorResponse,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}