using FarmsManager.Application.Common;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

namespace FarmsManager.Application.Interfaces;

public interface IContractorAutoCreationService : IService
{
    /// <summary>
    /// Ensures a contractor exists for the given NIP and module type.
    /// Creates one automatically if not found.
    /// </summary>
    /// <param name="nip">NIP of the contractor</param>
    /// <param name="name">Name of the contractor</param>
    /// <param name="address">Address of the contractor (optional)</param>
    /// <param name="moduleType">Module type to create contractor for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The contractor ID (existing or newly created)</returns>
    Task<Guid?> EnsureContractorExistsAsync(
        string nip,
        string name,
        string address,
        ModuleType moduleType,
        CancellationToken cancellationToken = default);
}
