using ILogger = Serilog.ILogger;

namespace FarmsManager.Api.Middleware;

public class PathTraversalProtectionMiddleware : IMiddleware
{
    private readonly ILogger _logger;
    
    // Lista podejrzanych ścieżek i rozszerzeń
    private static readonly string[] SuspiciousPaths =
    [
        ".env", ".git", ".aws", ".ssh", "web.config", "appsettings",
        "config.toml", "jwt.appsettings", "parameters.yml", 
        "client_secrets.json", "credentials", ".htaccess", ".htpasswd",
        "phpinfo", "info.php", "test.php", "backup", ".bak", ".sql",
        ".zip", ".tar", ".gz", "wp-config", "database.yml", "composer.json",
        "package.json", ".npmrc", ".dockerignore", "dockerfile"
    ];

    private static readonly string[] SuspiciousPatterns =
    [
        "..", "~", "%2e", "%00", "\\", "../", "..\\", 
        "/etc/", "/var/", "/proc/", "c:\\", "d:\\",
        "%2f", "%5c", "0x2f", "0x5c"
    ];

    public PathTraversalProtectionMiddleware(ILogger logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        var queryString = context.Request.QueryString.Value?.ToLowerInvariant() ?? string.Empty;
        var fullPath = path + queryString;

        // Sprawdź podejrzane ścieżki
        if (SuspiciousPaths.Any(suspicious => fullPath.Contains(suspicious, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.Warning(
                "Blocked suspicious path traversal attempt from {IP}: {Path} {QueryString}",
                context.Connection.RemoteIpAddress,
                context.Request.Path,
                context.Request.QueryString
            );
            
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        // Sprawdź podejrzane wzorce
        if (SuspiciousPatterns.Any(pattern => fullPath.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.Warning(
                "Blocked path traversal pattern from {IP}: {Path} {QueryString}",
                context.Connection.RemoteIpAddress,
                context.Request.Path,
                context.Request.QueryString
            );
            
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await next(context);
    }
}
