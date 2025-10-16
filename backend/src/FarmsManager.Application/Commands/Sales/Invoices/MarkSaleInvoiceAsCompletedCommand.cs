using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Sales.Invoices;

public record MarkSaleInvoiceAsCompletedCommandDto
{
    public DateOnly PaymentDate { get; init; }
    public string Comment { get; init; }
}

public record MarkSaleInvoiceAsCompletedCommand(Guid Id, MarkSaleInvoiceAsCompletedCommandDto Data) 
    : IRequest<EmptyBaseResponse>;

public class MarkSaleInvoiceAsCompletedCommandHandler 
    : IRequestHandler<MarkSaleInvoiceAsCompletedCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public MarkSaleInvoiceAsCompletedCommandHandler(
        IUserDataResolver userDataResolver,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(
        MarkSaleInvoiceAsCompletedCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        
        var saleInvoice = await _saleInvoiceRepository.GetAsync(
            new SaleInvoiceByIdSpec(request.Id), 
            cancellationToken);

        saleInvoice.MarkAsCompleted(request.Data.PaymentDate, request.Data.Comment);
        saleInvoice.SetModified(userId);
        
        await _saleInvoiceRepository.UpdateAsync(saleInvoice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class MarkSaleInvoiceAsCompletedCommandValidator : AbstractValidator<MarkSaleInvoiceAsCompletedCommand>
{
    public MarkSaleInvoiceAsCompletedCommandValidator()
    {
        RuleFor(x => x.Data.PaymentDate)
            .NotEmpty()
            .WithMessage("Data płatności jest wymagana.");
    }
}
