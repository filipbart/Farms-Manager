using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Gas.Deliveries;

public record UpdateGasDeliveryData
{
    public string InvoiceNumber { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Quantity { get; init; }
    public string Comment { get; init; }
}

public record UpdateGasDeliveryCommand(Guid Id, UpdateGasDeliveryData Data) : IRequest<EmptyBaseResponse>;

public class UpdateGasDeliveryCommandHandler : IRequestHandler<UpdateGasDeliveryCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;

    public UpdateGasDeliveryCommandHandler(IUserDataResolver userDataResolver,
        IGasDeliveryRepository gasDeliveryRepository)
    {
        _userDataResolver = userDataResolver;
        _gasDeliveryRepository = gasDeliveryRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateGasDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _gasDeliveryRepository.GetAsync(new GetGasDeliveryByIdSpec(request.Id),
                cancellationToken);

        entity.Update(
            request.Data.InvoiceNumber,
            request.Data.InvoiceDate,
            request.Data.UnitPrice * request.Data.Quantity,
            request.Data.UnitPrice,
            request.Data.Quantity,
            request.Data.Comment
        );

        entity.SetModified(userId);
        await _gasDeliveryRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateGasDeliveryCommandValidator : AbstractValidator<UpdateGasDeliveryCommand>
{
    public UpdateGasDeliveryCommandValidator()
    {
        RuleFor(t => t.Data.InvoiceNumber).NotEmpty();
        RuleFor(t => t.Data.InvoiceDate).NotEmpty();
        RuleFor(t => t.Data.UnitPrice).GreaterThan(0);
        RuleFor(t => t.Data.Quantity).GreaterThan(0);
    }
}