using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;

/// <summary>
/// Repository dla logów synchronizacji KSeF
/// </summary>
public interface IKSeFSynchronizationLogRepository : IRepository<KSeFSynchronizationLogEntity>;
