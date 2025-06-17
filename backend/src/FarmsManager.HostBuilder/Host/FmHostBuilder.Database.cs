using Autofac;
using FarmsManager.Infrastructure.Autofac;
using Microsoft.Extensions.Configuration;

namespace FarmsManager.HostBuilder.Host;

public partial class FmHostBuilder
{
    public FmHostBuilder AddDatabase()
    {
        CheckAutofacConfiguration();

        ConfigureContainer(builder =>
        {
            builder.RegisterModule(
                new FarmsManagerDatabaseModule(ConfigurationRoot.GetConnectionString("DefaultConnection") ?? throw new
                    NullReferenceException("Database connection string is not set.")));
        });

        return this;
    }
}