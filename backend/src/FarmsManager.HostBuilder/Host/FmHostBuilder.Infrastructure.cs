using Autofac;
using FarmsManager.Application.Common.Configurations;
using Microsoft.Extensions.Configuration;

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


        return this;
    }
}