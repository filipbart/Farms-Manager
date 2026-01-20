using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Accounting;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;

namespace FarmsManager.Application.Services;

public class PaymentStatusSynchronizationService : IPaymentStatusSynchronizationService
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public PaymentStatusSynchronizationService(
        IKSeFInvoiceRepository invoiceRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
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

        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<KSeFPaymentStatus?> GetModulePaymentStatusAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(
            new KSeFInvoiceByIdSpec(invoiceId), 
            cancellationToken);

        if (invoice == null || !invoice.AssignedEntityInvoiceId.HasValue)
        {
            return null;
        }

        var entityId = invoice.AssignedEntityInvoiceId.Value;

        switch (invoice.ModuleType)
        {
            case ModuleType.Feeds:
                var feedInvoice = await _feedInvoiceRepository.FirstOrDefaultAsync(
                    new Specifications.Feeds.GetFeedInvoiceByIdSpec(entityId),
                    cancellationToken);
                if (feedInvoice != null)
                {
                    return feedInvoice.PaymentId.HasValue 
                        ? KSeFPaymentStatus.PaidTransfer 
                        : KSeFPaymentStatus.Unpaid;
                }
                break;

            case ModuleType.Sales:
                var saleInvoice = await _saleInvoiceRepository.FirstOrDefaultAsync(
                    new Specifications.Sales.SaleInvoiceByIdSpec(entityId),
                    cancellationToken);
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
        var feedInvoice = await _feedInvoiceRepository.GetByIdAsync(entityId, cancellationToken);

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
        var saleInvoice = await _saleInvoiceRepository.GetByIdAsync(entityId, cancellationToken);

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
