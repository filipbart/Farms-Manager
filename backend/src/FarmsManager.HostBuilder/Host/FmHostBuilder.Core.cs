using FarmsManager.HostBuilder.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FarmsManager.HostBuilder.Host;

public partial class FmHostBuilder
{
    public static IConfiguration ConfigurationRoot { get; set; }
    public readonly IHostBuilder HostBuilder;
    public readonly IOptions<ConfigurationOptions> Configuration;

    protected internal FmHostBuilder(IHostBuilder hostBuilder)
    {
        HostBuilder = hostBuilder;
        ConfigurationRoot = LoadConfiguration();
        Configuration = Options.Create(ConfigurationRoot.Get<ConfigurationOptions>());
    }
}