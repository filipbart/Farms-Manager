using Microsoft.Extensions.Configuration;

namespace FarmsManager.HostBuilder.Host;

public partial class FmHostBuilder
{
    public static string BaseDirectory => Path.GetDirectoryName(typeof(FmHostBuilder).Assembly.Location);

    private IConfiguration LoadConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

        var files = Directory.GetFiles(BaseDirectory)
            .Where(t => t.Contains(".appsettings.json") || t.Contains("config.toml"))
            .ToList();

        configurationBuilder = files.Where(configFile => Path.GetExtension(configFile) == ".json")
            .Aggregate(configurationBuilder,
                (current, configFile) => current.AddJsonFile(configFile, optional: false, reloadOnChange: true));

        configurationBuilder = files.Where(configFile => Path.GetExtension(configFile) == ".toml")
            .Aggregate(configurationBuilder,
                (current, configFile) => current.AddTomlFile(configFile, optional: false, reloadOnChange: true));

        configurationBuilder = configurationBuilder.AddEnvironmentVariables();

        return configurationBuilder.Build();
    }
}