using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FarmsManager.Application.Common.Constants;
using FarmsManager.Domain.Aggregates.UserAggregate;
using FarmsManager.Domain.Aggregates.UserAggregate.Entites;
using FarmsManager.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FarmsManager.Infrastructure;

public class JwtUserAccessTokenGenerator : IUserAccessTokenGenerator
{
    private readonly IOptions<JwtOptions> _jwtOptions;

    public JwtUserAccessTokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        if (jwtOptions.Value == null)
            throw new ArgumentNullException(nameof(jwtOptions));
        if (string.IsNullOrEmpty(jwtOptions.Value.Issuer))
            throw new InvalidOperationException("JwtOptions.Issuer is not set");
        if (string.IsNullOrEmpty(jwtOptions.Value.Secret))
            throw new InvalidOperationException("JwtOptions.Secret is not set");

        _jwtOptions = jwtOptions;
    }


    public GenerateTokenResponse GenerateToken(UserEntity user)
    {
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimNames.UserId, user.Id.ToString(), null, _jwtOptions.Value.Issuer));

        claimsIdentity.AddClaim(new Claim(ClaimNames.Login, user.Login, null, _jwtOptions.Value.Issuer));

        var sessionId = Guid.NewGuid();
        claimsIdentity.AddClaim(new Claim(ClaimNames.SessionId, sessionId.ToString(), null, _jwtOptions.Value.Issuer));

        claimsIdentity.AddClaim(new Claim(ClaimNames.LongValid, _jwtOptions.Value.Expiry.ToString(), null,
            _jwtOptions.Value.Issuer));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var header = new JwtHeader(credentials);
        var now = DateTime.UtcNow;
        var expiry = new DateTimeOffset(now.Add(_jwtOptions.Value.Expiry)).ToUnixTimeSeconds();
        var issuedAt = new DateTimeOffset(now).ToUnixTimeSeconds();
        var payload = new JwtPayload(claimsIdentity.Claims)
        {
            { JwtKeys.Issuer, _jwtOptions.Value.Issuer },
            { JwtKeys.IssuedAt, issuedAt },
            { JwtKeys.Expiry, expiry }
        };

        var securityToken = new JwtSecurityToken(header, payload);
        var handler = new JwtSecurityTokenHandler();

        return new GenerateTokenResponse(handler.WriteToken(securityToken), sessionId, _jwtOptions.Value.Expiry, now);
    }
}