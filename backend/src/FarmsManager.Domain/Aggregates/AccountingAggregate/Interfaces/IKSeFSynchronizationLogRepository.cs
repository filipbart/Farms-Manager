using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;

/// <summary>
/// Repository dla logów synchronizacji KSeF
/// </summary>
public interface IKSeFSynchronizationLogRepository : IRepository<KSeFSynchronizationLogEntity>
{
    /// <summary>
    /// Pobiera ostatni log synchronizacji
    /// </summary>
    Task<KSeFSynchronizationLogEntity> GetLastSynchronizationAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pobiera historię synchronizacji (ostatnie N wpisów)
    /// </summary>
    Task<List<KSeFSynchronizationLogEntity>> GetSynchronizationHistoryAsync(int count, CancellationToken cancellationToken = default);
}
