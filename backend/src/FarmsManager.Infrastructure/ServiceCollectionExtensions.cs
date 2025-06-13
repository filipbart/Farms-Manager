using System.Text;
using FarmsManager.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FarmsManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwt(this IServiceCollection services, IOptions<JwtOptions> jwtOptions)
    {
        services.AddAuthentication(t =>
            {
                t.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                t.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                t.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Value.Issuer,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}