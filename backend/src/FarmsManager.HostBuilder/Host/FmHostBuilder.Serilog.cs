using Autofac;
using Serilog;
using Serilog.Events;

namespace FarmsManager.HostBuilder.Host;

public partial class FmHostBuilder
{
    public FmHostBuilder AddSerilog()
    {
        CheckAutofacConfiguration();

        const string serilogOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        var loggerBuilder = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: serilogOutputTemplate);

        if (Configuration.Value.Serilog.File.Enabled)
        {
            loggerBuilder = loggerBuilder.WriteTo.File(Configuration.Value.Serilog.File.Path,
                restrictedToMinimumLevel: LogEventLevel.Information, rollingInterval: RollingInterval.Day);
        }

        var logger = loggerBuilder.CreateLogger();
        Log.Logger = logger;

        ConfigureContainer(builder =>
        {
            builder.RegisterInstance(logger)
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
        });

        HostBuilder.UseSerilog(logger);

        return this;
    }
}