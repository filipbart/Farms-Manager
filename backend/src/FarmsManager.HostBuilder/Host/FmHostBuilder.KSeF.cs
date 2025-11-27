using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.KSeF;
using FarmsManager.Infrastructure.BackgroundJobs;
using KSeF.Client.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmsManager.HostBuilder.Host;

public partial class FmHostBuilder
{
    public FmHostBuilder AddKsEF()
    {
        CheckAutofacConfiguration();

        ConfigureServices(services =>
        {
            var kSeFOptions = ConfigurationRoot.GetSection("KSeF").Get<KSeFClientOptions>();
            services.AddKSeFClient(options => { options.BaseUrl = kSeFOptions?.BaseUrl ?? KsefEnviromentsUris.TEST; });
            services.AddCryptographyClient(options => { }, null);
            
            // Konfiguracja synchronizacji KSeF
            services.Configure<KSeFSyncConfiguration>(ConfigurationRoot.GetSection("KSeFSyncConfiguration"));
            
            // Rejestracja BackgroundService jako singleton (IKSeFSynchronizationJob) i jako HostedService
            services.AddSingleton<KSeFSynchronizationJob>();
            services.AddSingleton<IKSeFSynchronizationJob>(sp => sp.GetRequiredService<KSeFSynchronizationJob>());
            services.AddHostedService(sp => sp.GetRequiredService<KSeFSynchronizationJob>());
        });

        return this;
    }
}