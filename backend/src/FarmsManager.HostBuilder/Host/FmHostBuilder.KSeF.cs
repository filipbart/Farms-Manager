using Amazon;
using Amazon.S3;
using Autofac;
using KSeF.Client.Clients;
using KSeF.Client.DI;
using Microsoft.Extensions.Configuration;

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
        });

        return this;
    }
}