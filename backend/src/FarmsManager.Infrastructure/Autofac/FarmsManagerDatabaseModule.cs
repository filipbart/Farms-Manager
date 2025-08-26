using Autofac;
using FarmsManager.Domain.SeedWork;
using FarmsManager.Infrastructure.Repositories.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FarmsManager.Infrastructure.Autofac;

public class FarmsManagerDatabaseModule : Module
{
    private readonly string _connectionString;
    private static bool _loaded;

    public FarmsManagerDatabaseModule(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void Load(ContainerBuilder builder)
    {
        if (_loaded) return;
        _loaded = true;

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var npgsqlDataSource = dataSourceBuilder.Build();

        builder.Register(_ =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<FarmsManagerContext>()
                    .UseNpgsql(npgsqlDataSource, options =>
                    {
                        options.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    })
                    .UseLazyLoadingProxies()
                    .UseSnakeCaseNamingConvention()
                    .EnableSensitiveDataLogging();

                return new FarmsManagerContext(optionsBuilder.Options);
            })
            .As<DbContext>()
            .AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(typeof(IRepository).Assembly, typeof(UserRepository).Assembly)
            .Where(t => t.IsAssignableTo<IRepository>())
            .AsImplementedInterfaces();
    }
}