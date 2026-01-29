using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Accounting;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;

namespace FarmsManager.Application.Services;

public class InvoiceAuditService : IInvoiceAuditService
{
    private readonly IKSeFInvoiceAuditLogRepository _auditLogRepository;

    public InvoiceAuditService(IKSeFInvoiceAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task LogStatusChangeAsync(
        Guid invoiceId,
        KSeFInvoiceAuditAction action,
        KSeFInvoiceStatus? previousStatus,
        KSeFInvoiceStatus? newStatus,
        Guid userId,
        string userName,
        string comment = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = KSeFInvoiceAuditLogEntity.CreateNew(
            invoiceId: invoiceId,
            action: action,
            previousStatus: previousStatus,
            newStatus: newStatus,
            userId: userId,
            userName: userName,
            comment: comment
        );

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
        await _auditLogRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<KSeFInvoiceAuditLogEntity>> GetInvoiceAuditHistoryAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        return await _auditLogRepository.ListAsync(
            new KSeFInvoiceAuditLogByInvoiceIdSpec(invoiceId),
            cancellationToken);
    }

    public async Task<List<KSeFInvoiceAuditLogEntity>> GetAuditLogsByUserAsync(
        Guid userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        return await _auditLogRepository.ListAsync(
            new KSeFInvoiceAuditLogByUserIdSpec(userId, fromDate, toDate),
            cancellationToken);
    }
}
