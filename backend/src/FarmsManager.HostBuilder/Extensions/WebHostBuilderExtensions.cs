using Microsoft.AspNetCore.Hosting;

namespace FarmsManager.HostBuilder.Extensions;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder UsePortForHttp(this IWebHostBuilder builder, int port)
    {
        builder.UseKestrel(t => { t.ListenAnyIP(port); });

        return builder;
    }
}