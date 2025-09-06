using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FarmsManager.Api.Controllers;
using FarmsManager.Api.Middleware;
using FarmsManager.Application.Commands.Dev;
using FarmsManager.Application.Common;
using FarmsManager.Application.Mappings;
using FarmsManager.Application.Queries.User;
using FarmsManager.Application.Services;
using FarmsManager.HostBuilder.Extensions;
using FarmsManager.HostBuilder.Host;
using FarmsManager.Infrastructure;
using FarmsManager.Infrastructure.Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;

var webAppBuilder = WebApplication.CreateBuilder(args);

webAppBuilder.Host.GetBuilder()
    .AddAutofac(builder =>
    {
        builder.RegisterAssemblyTypes(typeof(DomainExceptionMiddleware).Assembly)
            .Where(t => t.IsAssignableTo<IMiddleware>()).AsSelf();
        builder.RegisterModule<InfrastructureModule>();

        builder.RegisterAssemblyTypes(typeof(AuthService).Assembly)
            .Where(t => t.IsAssignableTo<IService>())
            .AsImplementedInterfaces();
    })
    .AddDatabase()
    .AddInfrastructure()
    .AddJwt()
    .AddS3()
    .AddAutoMapper(typeof(UserProfile).Assembly)
    .AddMediator(typeof(MeQuery).Assembly, typeof(CreateDevAccountCommand).Assembly)
    .AddSerilog();

webAppBuilder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

webAppBuilder.WebHost.UsePortForHttp(FmHostBuilder.ConfigurationRoot.GetValue<int?>("AppPort") ?? 8082);

if (!webAppBuilder.Environment.IsProduction())
{
    webAppBuilder.Services.AddControllers()
        .AddApplicationPart(typeof(DevController).Assembly); // Dodaj kontroler tylko w środowisku innym niż produkcyjne
}

webAppBuilder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddResponseCompression()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "FarmsManager.Api.xml");
        if (File.Exists(filePath))
        {
            options.IncludeXmlComments(filePath);
        }

        options.MapType<TimeSpan>(() => new OpenApiSchema
        {
            Type = "string",
            Example = new OpenApiString("00:00:00")
        });

        options.DescribeAllParametersInCamelCase();

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    })
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

QuestPDF.Settings.License = LicenseType.Community;

var app = webAppBuilder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FarmsManagerContext>();
    context.Database.Migrate();
}

using (var lifetimeScope = app.Services.GetAutofacRoot().BeginLifetimeScope())
{
    var configuration = lifetimeScope.Resolve<IConfiguration>();

    if (configuration.GetValue<bool>("SwaggerEnabled"))
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}

app.UseCors(builder =>
{
    var allowedOrigins = app.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
    builder.AllowCredentials();
    if (allowedOrigins != null) builder.WithOrigins(allowedOrigins);
});

app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();

app.MapControllers();

app.UseMiddleware<DomainExceptionMiddleware>();
app.UseMiddleware<BlockDevControllerMiddleware>();
app.UseMiddleware<CheckPermissionMiddleware>();
app.UseMiddleware<FluentValidationExceptionMiddleware>();

app.Run();