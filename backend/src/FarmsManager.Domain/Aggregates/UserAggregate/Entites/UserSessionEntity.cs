using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.UserAggregate.Entites;

public class UserSessionEntity : Entity
{
    protected UserSessionEntity()
    {
    }

    public Guid UserId { get; init; }
    public Guid SessionId { get; init; }
    public DateTime LastSeenAtUtc { get; set; }
    public DateTime? DeactivatedAtUtc { get; set; }

    public void UpdateLastSeenAt()
    {
        if (LastSeenAtUtc < DateTime.UtcNow)
            LastSeenAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        DeactivatedAtUtc = DateTime.UtcNow;
    }

    public static UserSessionEntity Create(Guid userId, Guid sessionId)
    {
        return new UserSessionEntity
        {
            UserId = userId,
            SessionId = sessionId,
            LastSeenAtUtc = DateTime.UtcNow,
        };
    }
}