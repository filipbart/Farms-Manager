using Autofac;
using FarmsManager.Application.Common.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmsManager.HostBuilder.Host;

public partial class FmHostBuilder
{
    public FmHostBuilder AddInfrastructure()
    {
        CheckAutofacConfiguration();

        ConfigureContainer(builder =>
        {
            builder.Register(_ => ConfigurationRoot.GetSection("Irzplus").Get<IrzplusOptions>())
                .AsImplementedInterfaces().AsSelf();
        });

        ConfigureServices(services =>
        {
            services.AddHttpClient("NBP", client =>
            {
                client.BaseAddress = new Uri("https://api.nbp.pl/api/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
        });

        return this;
    }
}