using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record UpdateKSeFInvoiceCommand(Guid InvoiceId, UpdateKSeFInvoiceDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateKSeFInvoiceDto
{
    public KSeFInvoiceStatus? Status { get; set; }
    public KSeFPaymentStatus? PaymentStatus { get; set; }
    public DateOnly? PaymentDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public ModuleType? ModuleType { get; set; }
    public KSeFVatDeductionType? VatDeductionType { get; set; }
    public string Comment { get; set; }
    public Guid? FarmId { get; set; }
    public Guid? CycleId { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string RelatedInvoiceNumber { get; set; }
    public decimal? GrossAmount { get; set; }
    public decimal? NetAmount { get; set; }
    public decimal? VatAmount { get; set; }
}

public class UpdateKSeFInvoiceCommandHandler : IRequestHandler<UpdateKSeFInvoiceCommand, EmptyBaseResponse>
{
    private readonly IKSeFInvoiceRepository _repository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IInvoiceAuditService _auditService;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly IFeedPaymentRepository _feedPaymentRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;

    public UpdateKSeFInvoiceCommandHandler(
        IKSeFInvoiceRepository repository, 
        IUserDataResolver userDataResolver,
        IInvoiceAuditService auditService,
        IFeedInvoiceRepository feedInvoiceRepository,
        ISaleInvoiceRepository saleInvoiceRepository,
        IFeedPaymentRepository feedPaymentRepository,
        IExpenseProductionRepository expenseProductionRepository,
        IGasDeliveryRepository gasDeliveryRepository)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
        _auditService = auditService;
        _feedInvoiceRepository = feedInvoiceRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
        _feedPaymentRepository = feedPaymentRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateKSeFInvoiceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var userName = _userDataResolver.GetLoginAsync();
        var invoice = await _repository.GetAsync(new KSeFInvoiceByIdSpec(request.InvoiceId), cancellationToken);

        var previousStatus = invoice.Status;
        var previousPaymentStatus = invoice.PaymentStatus;

        invoice.Update(
            request.Data.Status,
            request.Data.PaymentStatus,
            request.Data.PaymentDate,
            request.Data.DueDate,
            request.Data.ModuleType,
            request.Data.VatDeductionType,
            request.Data.Comment,
            request.Data.FarmId,
            request.Data.CycleId,
            request.Data.AssignedUserId,
            request.Data.RelatedInvoiceNumber,
            request.Data.GrossAmount,
            request.Data.NetAmount,
            request.Data.VatAmount
        );

        invoice.SetModified(userId);
        await _repository.UpdateAsync(invoice, cancellationToken);

        // Loguj zmianę statusu faktury
        if (request.Data.Status.HasValue && request.Data.Status.Value != previousStatus)
        {
            var action = request.Data.Status.Value switch
            {
                KSeFInvoiceStatus.Rejected => KSeFInvoiceAuditAction.Rejected,
                KSeFInvoiceStatus.Accepted => KSeFInvoiceAuditAction.Accepted,
                _ => (KSeFInvoiceAuditAction?)null
            };
            if (action.HasValue)
            {
                await _auditService.LogStatusChangeAsync(
                    invoice.Id,
                    action.Value,
                    previousStatus,
                    request.Data.Status.Value,
                    userId,
                    userName,
                    cancellationToken: cancellationToken);
            }
        }

        // Loguj zmianę statusu płatności
        if (request.Data.PaymentStatus.HasValue && request.Data.PaymentStatus.Value != previousPaymentStatus)
        {
            await _auditService.LogStatusChangeAsync(
                invoice.Id,
                KSeFInvoiceAuditAction.PaymentStatusChanged,
                previousStatus,
                invoice.Status,
                userId,
                userName,
                $"Zmiana statusu płatności: {previousPaymentStatus} → {request.Data.PaymentStatus.Value}",
                cancellationToken);
        }

        // Aktualizuj termin płatności w powiązanej encji modułowej (jeśli istnieje)
        if (request.Data.DueDate.HasValue && invoice.AssignedEntityInvoiceId.HasValue)
        {
            await UpdateModuleEntityDueDateAsync(invoice.ModuleType, invoice.AssignedEntityInvoiceId.Value, request.Data.DueDate.Value, cancellationToken);
        }

        // Synchronizuj komentarz do powiązanej encji modułowej (jeśli istnieje)
        if (!string.IsNullOrEmpty(request.Data.Comment) && invoice.AssignedEntityInvoiceId.HasValue)
        {
            await UpdateModuleEntityCommentAsync(invoice.ModuleType, invoice.AssignedEntityInvoiceId.Value, request.Data.Comment, cancellationToken);
        }

        // Synchronizuj status płatności do modułowych encji (Feeds i Sales)
        if (request.Data.PaymentStatus.HasValue && invoice.AssignedEntityInvoiceId.HasValue)
        {
            if (invoice.ModuleType == ModuleType.Feeds)
            {
                await UpdateFeedInvoicePaymentStatusAsync(
                    invoice.AssignedEntityInvoiceId.Value, 
                    request.Data.PaymentStatus.Value, 
                    request.Data.PaymentDate ?? invoice.PaymentDate,
                    invoice.FarmId ?? Guid.Empty,
                    invoice.AssignedCycleId ?? Guid.Empty,
                    userId,
                    cancellationToken);
            }
            else if (invoice.ModuleType == ModuleType.Sales)
            {
                await UpdateSaleInvoicePaymentStatusAsync(
                    invoice.AssignedEntityInvoiceId.Value, 
                    request.Data.PaymentStatus.Value, 
                    request.Data.PaymentDate ?? invoice.PaymentDate, 
                    cancellationToken);
            }
        }

        return BaseResponse.EmptyResponse;
    }

    private async Task UpdateModuleEntityDueDateAsync(ModuleType moduleType, Guid entityId, DateOnly dueDate, CancellationToken cancellationToken)
    {
        switch (moduleType)
        {
            case ModuleType.Feeds:
                var feedInvoice = await _feedInvoiceRepository.GetByIdAsync(entityId, cancellationToken);
                if (feedInvoice != null)
                {
                    feedInvoice.Update(
                        feedInvoice.InvoiceNumber,
                        feedInvoice.BankAccountNumber,
                        feedInvoice.ItemName,
                        feedInvoice.VendorName,
                        feedInvoice.Quantity,
                        feedInvoice.UnitPrice,
                        feedInvoice.InvoiceDate,
                        dueDate,
                        feedInvoice.InvoiceTotal,
                        feedInvoice.SubTotal,
                        feedInvoice.VatAmount,
                        feedInvoice.Comment);
                    await _feedInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                break;

            case ModuleType.Sales:
                var saleInvoice = await _saleInvoiceRepository.GetByIdAsync(entityId, cancellationToken);
                if (saleInvoice != null)
                {
                    saleInvoice.Update(
                        saleInvoice.InvoiceNumber,
                        saleInvoice.InvoiceDate,
                        dueDate,
                        saleInvoice.PaymentDate,
                        saleInvoice.InvoiceTotal,
                        saleInvoice.SubTotal,
                        saleInvoice.VatAmount,
                        saleInvoice.Comment);
                    await _saleInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                break;
        }
    }

    private async Task UpdateFeedInvoicePaymentStatusAsync(
        Guid feedInvoiceId, 
        KSeFPaymentStatus paymentStatus, 
        DateOnly? paymentDate,
        Guid farmId,
        Guid cycleId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var feedInvoice = await _feedInvoiceRepository.GetByIdAsync(feedInvoiceId, cancellationToken);
        if (feedInvoice == null)
            return;

        // Jeśli faktura jest oznaczona jako opłacona (gotówka lub przelew)
        if (paymentStatus == KSeFPaymentStatus.PaidCash || paymentStatus == KSeFPaymentStatus.PaidTransfer)
        {
            // Jeśli faktura nie ma jeszcze przypisanego PaymentId, utwórz nowy FeedPayment
            if (!feedInvoice.PaymentId.HasValue)
            {
                var payment = FeedPaymentEntity.CreateNew(
                    farmId,
                    cycleId,
                    "Przelew z księgowości",
                    string.Empty,
                    userId);
                payment.MarkAsCompleted($"Automatycznie utworzony z księgowości w dniu {DateTime.Now:yyyy-MM-dd}");
                await _feedPaymentRepository.AddAsync(payment, cancellationToken);
                await _feedPaymentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                feedInvoice.MarkAsPaid(payment.Id);
                await _feedInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        // Jeśli faktura jest oznaczona jako nieopłacona
        else if (paymentStatus == KSeFPaymentStatus.Unpaid)
        {
            feedInvoice.MarkAsUnpaid();
            await _feedInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task UpdateSaleInvoicePaymentStatusAsync(Guid saleInvoiceId, KSeFPaymentStatus paymentStatus, DateOnly? paymentDate, CancellationToken cancellationToken)
    {
        var saleInvoice = await _saleInvoiceRepository.GetByIdAsync(saleInvoiceId, cancellationToken);
        if (saleInvoice == null)
            return;

        // Jeśli faktura jest oznaczona jako opłacona (gotówka lub przelew)
        if (paymentStatus == KSeFPaymentStatus.PaidCash || paymentStatus == KSeFPaymentStatus.PaidTransfer)
        {
            var effectivePaymentDate = paymentDate ?? DateOnly.FromDateTime(DateTime.Today);
            saleInvoice.MarkAsCompleted(effectivePaymentDate);
        }
        // Jeśli faktura jest oznaczona jako nieopłacona
        else if (paymentStatus == KSeFPaymentStatus.Unpaid)
        {
            saleInvoice.MarkAsUnrealized();
        }
        await _saleInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateModuleEntityCommentAsync(ModuleType moduleType, Guid entityId, string comment, CancellationToken cancellationToken)
    {
        switch (moduleType)
        {
            case ModuleType.Feeds:
                var feedInvoice = await _feedInvoiceRepository.GetByIdAsync(entityId, cancellationToken);
                if (feedInvoice != null)
                {
                    feedInvoice.SetComment(comment);
                    await _feedInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                break;

            case ModuleType.Sales:
                var saleInvoice = await _saleInvoiceRepository.GetByIdAsync(entityId, cancellationToken);
                if (saleInvoice != null)
                {
                    saleInvoice.Update(
                        saleInvoice.InvoiceNumber,
                        saleInvoice.InvoiceDate,
                        saleInvoice.DueDate,
                        saleInvoice.PaymentDate,
                        saleInvoice.InvoiceTotal,
                        saleInvoice.SubTotal,
                        saleInvoice.VatAmount,
                        comment);
                    await _saleInvoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                break;

            case ModuleType.ProductionExpenses:
                var expenseProduction = await _expenseProductionRepository.GetByIdAsync(entityId, cancellationToken);
                if (expenseProduction != null)
                {
                    expenseProduction.Update(
                        expenseProduction.InvoiceNumber,
                        expenseProduction.InvoiceTotal,
                        expenseProduction.SubTotal,
                        expenseProduction.VatAmount,
                        expenseProduction.InvoiceDate,
                        comment);
                    await _expenseProductionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                break;

            case ModuleType.Gas:
                var gasDelivery = await _gasDeliveryRepository.GetByIdAsync(entityId, cancellationToken);
                if (gasDelivery != null)
                {
                    gasDelivery.Update(
                        gasDelivery.InvoiceNumber,
                        gasDelivery.InvoiceDate,
                        gasDelivery.InvoiceTotal,
                        gasDelivery.UnitPrice,
                        gasDelivery.Quantity,
                        comment);
                    await _gasDeliveryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                break;
        }
    }
}
