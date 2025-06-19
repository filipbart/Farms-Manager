using FarmsManager.Application.Common;
using FarmsManager.Domain.Aggregates.UserAggregate;
using FarmsManager.Domain.Aggregates.UserAggregate.Entites;

namespace FarmsManager.Application.Interfaces;

public interface IUserSessionService : IService
{
    Task<GenerateTokenResponse> GenerateToken(UserEntity user, CancellationToken ct);
    Task DeactivateSessionAsync(CancellationToken ct);
}