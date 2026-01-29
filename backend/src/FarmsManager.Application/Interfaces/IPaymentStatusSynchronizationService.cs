using FarmsManager.Application.Common;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

namespace FarmsManager.Application.Interfaces;

public interface IPaymentStatusSynchronizationService : IService
{
    /// <summary>
    /// Synchronizes payment status from module entity (Feeds, Sales, etc.) to accounting invoice
    /// </summary>
    Task SyncPaymentStatusToAccountingAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes payment status from accounting invoice to module entity
    /// </summary>
    Task SyncPaymentStatusFromAccountingAsync(
        Guid invoiceId,
        KSeFPaymentStatus newPaymentStatus,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current payment status from the linked module entity
    /// </summary>
    Task<KSeFPaymentStatus?> GetModulePaymentStatusAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);
}
