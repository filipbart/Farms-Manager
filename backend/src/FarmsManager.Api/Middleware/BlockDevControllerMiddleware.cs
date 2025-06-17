namespace FarmsManager.Api.Middleware;

public class BlockDevControllerMiddleware : IMiddleware
{
    private readonly IWebHostEnvironment _env;

    public BlockDevControllerMiddleware(IWebHostEnvironment env)
    {
        _env = env;
    }


    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_env.IsProduction())
        {
            if (context.Request.Path.StartsWithSegments("/api/dev"))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
        }

        await next(context);
    }
}