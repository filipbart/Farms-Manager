using Autofac;
using FarmsManager.Domain.Aggregates.UserAggregate;
using FarmsManager.Infrastructure;
using FarmsManager.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;

namespace FarmsManager.HostBuilder.Host;

public partial class FmHostBuilder
{
    public FmHostBuilder AddJwt()
    {
        CheckAutofacConfiguration();

        ConfigureContainer(builder =>
        {
            builder.RegisterType<JwtUserAccessTokenGenerator>().As<IUserAccessTokenGenerator>();
            builder.Register(_ => ConfigurationRoot.GetSection("Jwt").Get<JwtOptions>())
                .AsImplementedInterfaces().AsSelf();
        });

        ConfigureServices(services => { services.AddJwt(ConfigurationRoot.GetSection("Jwt").Get<JwtOptions>()); });

        return this;
    }
}