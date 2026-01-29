using FarmsManager.Application.Common;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

namespace FarmsManager.Application.Interfaces;

public interface IInvoiceAuditService : IService
{
    Task LogStatusChangeAsync(
        Guid invoiceId,
        KSeFInvoiceAuditAction action,
        KSeFInvoiceStatus? previousStatus,
        KSeFInvoiceStatus? newStatus,
        Guid userId,
        string userName,
        string comment = null,
        CancellationToken cancellationToken = default);

    Task<List<KSeFInvoiceAuditLogEntity>> GetInvoiceAuditHistoryAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    Task<List<KSeFInvoiceAuditLogEntity>> GetAuditLogsByUserAsync(
        Guid userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
}
