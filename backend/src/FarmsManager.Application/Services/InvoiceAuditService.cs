using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Services;

public class InvoiceAuditService : IInvoiceAuditService
{
    private readonly IKSeFInvoiceAuditLogRepository _auditLogRepository;
    private readonly DbContext _dbContext;

    public InvoiceAuditService(IKSeFInvoiceAuditLogRepository auditLogRepository, DbContext dbContext)
    {
        _auditLogRepository = auditLogRepository;
        _dbContext = dbContext;
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
        return await _dbContext.Set<KSeFInvoiceAuditLogEntity>()
            .Where(a => a.InvoiceId == invoiceId && a.DateDeletedUtc == null)
            .OrderByDescending(a => a.DateCreatedUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<KSeFInvoiceAuditLogEntity>> GetAuditLogsByUserAsync(
        Guid userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<KSeFInvoiceAuditLogEntity>()
            .Where(a => a.UserId == userId && a.DateDeletedUtc == null);

        if (fromDate.HasValue)
            query = query.Where(a => a.DateCreatedUtc >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.DateCreatedUtc <= toDate.Value);

        return await query
            .OrderByDescending(a => a.DateCreatedUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
