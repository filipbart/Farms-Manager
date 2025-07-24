using Amazon;
using Amazon.S3;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace FarmsManager.HostBuilder.Host;

public partial class FmHostBuilder
{
    public FmHostBuilder AddS3()
    {
        CheckAutofacConfiguration();

        ConfigureContainer(builder =>
        {
            builder.Register(t =>
            {
                var ctx = t.Resolve<IComponentContext>();
                var configuration = ctx.Resolve<IConfiguration>();

                var url = configuration.GetValue<string>("S3:Url");
                var accessKey = configuration.GetValue<string>("S3:AccessKey");
                var secretKey = configuration.GetValue<string>("S3:SecretKey");
                var authRegion = configuration.GetValue<string>("S3:AuthRegion");
                var region = configuration.GetValue<string>("S3:region");

                var config = new AmazonS3Config
                {
                    ServiceURL = url,
                    UseHttp = true,
                    ForcePathStyle = true,
                    AuthenticationRegion = authRegion,
                    RegionEndpoint = RegionEndpoint.GetBySystemName(region)
                };

                if (configuration.GetValue<bool>("TestMode"))
                {
                    config.ServiceURL = url;
                }

                return new AmazonS3Client(accessKey, secretKey, config);
            }).AsSelf().AsImplementedInterfaces().SingleInstance();
        });


        return this;
    }
}