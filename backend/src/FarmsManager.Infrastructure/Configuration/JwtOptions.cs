using Microsoft.Extensions.Options;

namespace FarmsManager.Infrastructure.Configuration;

public class JwtOptions : IOptions<JwtOptions>
{
    public const string Key = "Jwt";

    public string Issuer { get; init; }

    public string Secret { get; init; }

    public string SercetLoginNotifications { get; init; }

    public TimeSpan Expiry { get; init; }
    public JwtOptions Value => this;
}