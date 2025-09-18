using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Application.Specifications.Cycle;

namespace FarmsManager.Application.Commands.Sales.Invoices;

public record UpdateSalesInvoiceData
{
    public Guid CycleId { get; init; }
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
    private readonly ICycleRepository _cycleRepository;

    public UpdateSalesInvoiceCommandHandler(IUserDataResolver userDataResolver,
        ISaleInvoiceRepository saleInvoiceRepository, ICycleRepository cycleRepository)
    {
        _userDataResolver = userDataResolver;
        _saleInvoiceRepository = saleInvoiceRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateSaleInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _saleInvoiceRepository.GetAsync(new SaleInvoiceByIdSpec(request.Id),
                cancellationToken);

        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), cancellationToken);

        if (entity.CycleId != cycle.Id)
        {
            entity.SetCycle(cycle.Id);
        }

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
        RuleFor(t => t.Data.CycleId).NotEmpty();
        RuleFor(t => t.Data.InvoiceNumber).NotEmpty();
        RuleFor(t => t.Data.InvoiceTotal).GreaterThan(0);
        RuleFor(t => t.Data.SubTotal).GreaterThan(0);
        RuleFor(t => t.Data.VatAmount).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.InvoiceDate).NotEmpty();
        RuleFor(t => t.Data.DueDate).NotEmpty();
    }
}