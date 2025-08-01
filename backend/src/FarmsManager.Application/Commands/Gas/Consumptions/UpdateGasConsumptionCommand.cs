using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Gas.Consumptions;

public record UpdateGasConsumptionDto
{
    public decimal QuantityConsumed { get; init; }
}

public record UpdateGasConsumptionCommand(Guid Id, UpdateGasConsumptionDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateGasConsumptionCommandHandler : IRequestHandler<UpdateGasConsumptionCommand, EmptyBaseResponse>
{
    private readonly IMediator _mediator;
    private readonly IGasConsumptionRepository _repository;

    public UpdateGasConsumptionCommandHandler(IMediator mediator, IGasConsumptionRepository repository)
    {
        _mediator = mediator;
        _repository = repository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateGasConsumptionCommand request,
        CancellationToken cancellationToken)
    {
        var originalConsumption = await _repository.GetByIdAsync(request.Id, cancellationToken)
                                  ?? throw new Exception("Nie znaleziono wpisu zużycia o podanym ID.");

        await _mediator.Send(new DeleteGasConsumptionCommand(request.Id), cancellationToken);

        var addCommand = new AddGasConsumptionCommand
        {
            FarmId = originalConsumption.FarmId,
            CycleId = originalConsumption.CycleId,
            QuantityConsumed = request.Data.QuantityConsumed
        };
        await _mediator.Send(addCommand, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class UpdateGasConsumptionCommandValidator : AbstractValidator<UpdateGasConsumptionCommand>
{
    public UpdateGasConsumptionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.QuantityConsumed).GreaterThan(0);
    }
}