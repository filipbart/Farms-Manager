using FarmsManager.Domain.Aggregates.UserAggregate.Entities;

namespace FarmsManager.Domain.Aggregates.UserAggregate;

public interface IUserAccessTokenGenerator
{
    GenerateTokenResponse GenerateToken(UserEntity user);
}

public class GenerateTokenResponse(string token, Guid sessionId, TimeSpan validFor, DateTime generationDate)
{
    public string Token { get; } = token;
    public Guid SessionId { get; } = sessionId;
    public TimeSpan ValidFor { get; } = validFor;
    public DateTime GenerationDate { get; } = generationDate;
}