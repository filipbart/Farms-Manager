using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Sales.Invoices;

public record UpdateSalesInvoiceData
{
    public string InvoiceNumber { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public DateOnly? PaymentDate { get; init; }
}

public record UpdateSaleInvoiceCommand(Guid Id, UpdateSalesInvoiceData Data) : IRequest<EmptyBaseResponse>;

public class UpdateSalesInvoiceCommandHandler : IRequestHandler<UpdateSaleInvoiceCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public UpdateSalesInvoiceCommandHandler(IUserDataResolver userDataResolver,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateSaleInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _saleInvoiceRepository.GetAsync(new SaleInvoiceByIdSpec(request.Id),
                cancellationToken);

        entity.Update(
            request.Data.InvoiceNumber,
            request.Data.InvoiceDate,
            request.Data.DueDate,
            request.Data.PaymentDate,
            request.Data.InvoiceTotal,
            request.Data.SubTotal,
            request.Data.VatAmount
        );

        entity.SetModified(userId);
        await _saleInvoiceRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateSalesInvoiceCommandValidator : AbstractValidator<UpdateSaleInvoiceCommand>
{
    public UpdateSalesInvoiceCommandValidator()
    {
        RuleFor(t => t.Data.InvoiceNumber).NotEmpty();
        RuleFor(t => t.Data.InvoiceTotal).GreaterThan(0);
        RuleFor(t => t.Data.SubTotal).GreaterThan(0);
        RuleFor(t => t.Data.VatAmount).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.InvoiceDate).NotEmpty();
        RuleFor(t => t.Data.DueDate).NotEmpty();
    }
}