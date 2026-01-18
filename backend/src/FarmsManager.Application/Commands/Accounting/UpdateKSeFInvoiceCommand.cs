using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record UpdateKSeFInvoiceCommand(Guid InvoiceId, UpdateKSeFInvoiceDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateKSeFInvoiceDto
{
    public KSeFInvoiceStatus? Status { get; set; }
    public KSeFPaymentStatus? PaymentStatus { get; set; }
    public ModuleType? ModuleType { get; set; }
    public KSeFVatDeductionType? VatDeductionType { get; set; }
    public string Comment { get; set; }
    public Guid? FarmId { get; set; }
    public Guid? CycleId { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string RelatedInvoiceNumber { get; set; }
}

public class UpdateKSeFInvoiceCommandHandler : IRequestHandler<UpdateKSeFInvoiceCommand, EmptyBaseResponse>
{
    private readonly IKSeFInvoiceRepository _repository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IInvoiceAuditService _auditService;

    public UpdateKSeFInvoiceCommandHandler(
        IKSeFInvoiceRepository repository, 
        IUserDataResolver userDataResolver,
        IInvoiceAuditService auditService)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
        _auditService = auditService;
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
            request.Data.ModuleType,
            request.Data.VatDeductionType,
            request.Data.Comment,
            request.Data.FarmId,
            request.Data.CycleId,
            request.Data.AssignedUserId,
            request.Data.RelatedInvoiceNumber
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
                KSeFInvoiceStatus.SentToOffice => KSeFInvoiceAuditAction.TransferredToOffice,
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

        return BaseResponse.EmptyResponse;
    }
}
