using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Services;

public class PaymentStatusSynchronizationService : IPaymentStatusSynchronizationService
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly DbContext _dbContext;

    public PaymentStatusSynchronizationService(
        IKSeFInvoiceRepository invoiceRepository,
        DbContext dbContext)
    {
        _invoiceRepository = invoiceRepository;
        _dbContext = dbContext;
    }

    public async Task SyncPaymentStatusToAccountingAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken)
            ?? throw DomainException.RecordNotFound($"Invoice {invoiceId} not found");

        if (!invoice.AssignedEntityInvoiceId.HasValue)
        {
            return;
        }

        var modulePaymentStatus = await GetModulePaymentStatusAsync(invoiceId, cancellationToken);
        
        if (modulePaymentStatus.HasValue && modulePaymentStatus.Value != invoice.PaymentStatus)
        {
            invoice.Update(paymentStatus: modulePaymentStatus.Value);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SyncPaymentStatusFromAccountingAsync(
        Guid invoiceId,
        KSeFPaymentStatus newPaymentStatus,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken)
            ?? throw DomainException.RecordNotFound($"Invoice {invoiceId} not found");

        if (!invoice.AssignedEntityInvoiceId.HasValue)
        {
            return;
        }

        var entityId = invoice.AssignedEntityInvoiceId.Value;
        var isPaid = newPaymentStatus == KSeFPaymentStatus.PaidCash || 
                     newPaymentStatus == KSeFPaymentStatus.PaidTransfer;

        switch (invoice.ModuleType)
        {
            case ModuleType.Feeds:
                await SyncToFeedInvoiceAsync(entityId, isPaid, cancellationToken);
                break;
            case ModuleType.Sales:
                await SyncToSaleInvoiceAsync(entityId, isPaid, cancellationToken);
                break;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<KSeFPaymentStatus?> GetModulePaymentStatusAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _dbContext.Set<KSeFInvoiceEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);

        if (invoice == null || !invoice.AssignedEntityInvoiceId.HasValue)
        {
            return null;
        }

        var entityId = invoice.AssignedEntityInvoiceId.Value;

        switch (invoice.ModuleType)
        {
            case ModuleType.Feeds:
                var feedInvoice = await _dbContext.Set<FeedInvoiceEntity>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == entityId, cancellationToken);
                if (feedInvoice != null)
                {
                    return feedInvoice.PaymentId.HasValue 
                        ? KSeFPaymentStatus.PaidTransfer 
                        : KSeFPaymentStatus.Unpaid;
                }
                break;

            case ModuleType.Sales:
                var saleInvoice = await _dbContext.Set<SaleInvoiceEntity>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == entityId, cancellationToken);
                if (saleInvoice != null)
                {
                    return saleInvoice.PaymentDate.HasValue 
                        ? KSeFPaymentStatus.PaidTransfer 
                        : KSeFPaymentStatus.Unpaid;
                }
                break;
        }

        return null;
    }

    private async Task SyncToFeedInvoiceAsync(Guid entityId, bool isPaid, CancellationToken cancellationToken)
    {
        var feedInvoice = await _dbContext.Set<FeedInvoiceEntity>()
            .FirstOrDefaultAsync(f => f.Id == entityId, cancellationToken);

        if (feedInvoice == null) return;

        if (isPaid && !feedInvoice.PaymentId.HasValue)
        {
            feedInvoice.MarkAsPaid(Guid.NewGuid());
        }
        else if (!isPaid && feedInvoice.PaymentId.HasValue)
        {
            feedInvoice.MarkAsUnpaid();
        }
    }

    private async Task SyncToSaleInvoiceAsync(Guid entityId, bool isPaid, CancellationToken cancellationToken)
    {
        var saleInvoice = await _dbContext.Set<SaleInvoiceEntity>()
            .FirstOrDefaultAsync(s => s.Id == entityId, cancellationToken);

        if (saleInvoice == null) return;

        if (isPaid && !saleInvoice.PaymentDate.HasValue)
        {
            saleInvoice.MarkAsCompleted(DateOnly.FromDateTime(DateTime.UtcNow));
        }
        else if (!isPaid && saleInvoice.PaymentDate.HasValue)
        {
            saleInvoice.MarkAsUnrealized();
        }
    }
}
