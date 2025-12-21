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

    public UpdateKSeFInvoiceCommandHandler(IKSeFInvoiceRepository repository, IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateKSeFInvoiceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var invoice = await _repository.GetAsync(new KSeFInvoiceByIdSpec(request.InvoiceId), cancellationToken);

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

        return BaseResponse.EmptyResponse;
    }
}
