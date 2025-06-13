using FarmsManager.HostBuilder.Host;
using Microsoft.Extensions.Hosting;

namespace FarmsManager.HostBuilder.Extensions;

public static class HostBuilderExtensions
{
    public static FmHostBuilder GetBuilder(this IHostBuilder builder) => new(builder);
}