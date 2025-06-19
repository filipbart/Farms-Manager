using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.UserAggregate.Entites;

namespace FarmsManager.Application.Specifications.UserSessions;

public sealed class UserSessionBySessionIdSpec : BaseSpecification<UserSessionEntity>,
    ISingleResultSpecification<UserSessionEntity>
{
    public UserSessionBySessionIdSpec(Guid sessionId)
    {
        EnsureExists();
        Query.Where(t => t.DeactivatedAtUtc.HasValue == false);
        Query.Where(t => t.SessionId == sessionId);
    }
}