using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FarmsManager.HostBuilder.Host;

public partial class FmHostBuilder
{
    private bool _isContainerConfigured;

    protected void CheckAutofacConfiguration()
    {
        if (_isContainerConfigured == false)
        {
            throw new Exception("Autofac configuration is not set. Use AddAutofac() method to set it.");
        }
    }

    public void ConfigureServices(Action<IServiceCollection> method)
    {
        HostBuilder.ConfigureServices(method);
    }

    public void ConfigureContainer(Action<ContainerBuilder> builderAction)
    {
        CheckAutofacConfiguration();

        HostBuilder.ConfigureContainer<ContainerBuilder>((_, builder) => { builderAction(builder); });
    }

    public FmHostBuilder AddAutofac(Action<ContainerBuilder> builderAction = null)
    {
        if (_isContainerConfigured) return this;

        HostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        HostBuilder.ConfigureContainer<ContainerBuilder>((_, builder) =>
        {
            builder.RegisterInstance(Configuration).AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterInstance(ConfigurationRoot).As<IConfiguration>().SingleInstance();

            builderAction?.Invoke(builder);
        });

        _isContainerConfigured = true;

        return this;
    }

    public FmHostBuilder AddMediator(params Assembly[] assemblies)
    {
        CheckAutofacConfiguration();

        var conf = MediatRConfigurationBuilder.Create(assemblies)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        ConfigureContainer(builder => { builder.RegisterMediatR(conf); });

        return this;
    }

    public FmHostBuilder AddAutoMapper(params Assembly[] assemblies)
    {
        CheckAutofacConfiguration();

        ConfigureContainer(builder =>
        {
            var assembliesList = assemblies.ToList();
            builder.RegisterAutoMapper(true, assembliesList.ToArray());
        });

        return this;
    }
}