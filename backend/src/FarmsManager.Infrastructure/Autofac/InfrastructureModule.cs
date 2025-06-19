using Autofac;
using FarmsManager.Application.Services;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Infrastructure.Autofac;

public class InfrastructureModule : Module
{
    private static bool _loaded;

    protected override void Load(ContainerBuilder builder)
    {
        if (_loaded)
            return;

        _loaded = true;

        builder.RegisterType<HttpContextAccessor>().AsImplementedInterfaces();
        builder.RegisterType<UserDataResolver>().AsImplementedInterfaces();
    }
}