using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Gas.Consumptions;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Domain.SeedWork;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Gas.Consumptions;

public record AddGasConsumptionCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public decimal QuantityConsumed { get; init; }
}

public class AddGasConsumptionCommandHandler : IRequestHandler<AddGasConsumptionCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IGasConsumptionRepository _gasConsumptionRepository;
    private readonly IRepository<GasConsumptionSourceEntity> _consumptionSourceRepository;

    public AddGasConsumptionCommandHandler(
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IGasDeliveryRepository gasDeliveryRepository,
        IGasConsumptionRepository gasConsumptionRepository,
        IRepository<GasConsumptionSourceEntity> consumptionSourceRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _gasConsumptionRepository = gasConsumptionRepository;
        _consumptionSourceRepository = consumptionSourceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddGasConsumptionCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);

        var spec = new GetAvailableGasDeliveriesForFarmSpec(request.FarmId);
        var availableDeliveries = await _gasDeliveryRepository.ListAsync(spec, ct);

        var quantityToConsume = request.QuantityConsumed;
        decimal totalCost = 0;

        var sourceData = new List<(Guid GasDeliveryId, decimal ConsumedQuantity)>();
        var deliveriesToUpdate = new List<GasDeliveryEntity>();

        foreach (var delivery in availableDeliveries)
        {
            if (quantityToConsume <= 0) break;

            var availableInDelivery = delivery.Quantity - delivery.UsedQuantity;
            var quantityToTake = Math.Min(quantityToConsume, availableInDelivery);

            delivery.AddUsedQuantity(quantityToTake);
            deliveriesToUpdate.Add(delivery);

            totalCost += quantityToTake * delivery.UnitPrice;

            sourceData.Add((delivery.Id, quantityToTake));

            quantityToConsume -= quantityToTake;
        }

        if (quantityToConsume > 0)
        {
            throw new Exception("Niewystarczająca ilość gazu w dostępnych dostawach, aby pokryć podane zużycie.");
        }

        var newConsumption = GasConsumptionEntity.CreateNew(
            request.FarmId,
            request.CycleId,
            request.QuantityConsumed,
            Math.Round(totalCost, 2),
            userId
        );

        var sourcesToCreate = sourceData
            .Select(s => new GasConsumptionSourceEntity
            {
                GasConsumptionId = newConsumption.Id,
                GasDeliveryId = s.GasDeliveryId,
                ConsumedQuantity = s.ConsumedQuantity
            })
            .ToList();

        await _gasConsumptionRepository.AddAsync(newConsumption, ct);
        await _consumptionSourceRepository.AddRangeAsync(sourcesToCreate, ct);
        await _gasDeliveryRepository.UpdateRangeAsync(deliveriesToUpdate, ct);

        return BaseResponse.EmptyResponse;
    }
}

public class AddGasConsumptionCommandValidator : AbstractValidator<AddGasConsumptionCommand>
{
    public AddGasConsumptionCommandValidator()
    {
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.CycleId).NotEmpty();
        RuleFor(t => t.QuantityConsumed).GreaterThan(0);
    }
}