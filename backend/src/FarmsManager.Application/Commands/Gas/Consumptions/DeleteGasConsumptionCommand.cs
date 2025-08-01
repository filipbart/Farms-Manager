using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Enum;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Gas.Consumptions;

public record DeleteGasConsumptionCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteGasConsumptionCommandHandler : IRequestHandler<DeleteGasConsumptionCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IGasConsumptionRepository _consumptionRepository;
    private readonly IGasDeliveryRepository _deliveryRepository;
    private readonly IGasConsumptionSourceRepository _consumptionSourceRepository;

    public DeleteGasConsumptionCommandHandler(
        IUserDataResolver userDataResolver,
        IGasConsumptionRepository consumptionRepository,
        IGasDeliveryRepository deliveryRepository,
        IGasConsumptionSourceRepository consumptionSourceRepository)
    {
        _userDataResolver = userDataResolver;
        _consumptionRepository = consumptionRepository;
        _deliveryRepository = deliveryRepository;
        _consumptionSourceRepository = consumptionSourceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteGasConsumptionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var originalConsumption =
            await _consumptionRepository.GetAsync(new GetGasConsumptionByIdSpec(request.Id),
                cancellationToken);

        if (originalConsumption.Status == GasConsumptionStatus.Cancelled)
        {
            throw new Exception("Ten wpis został już anulowany.");
        }

        originalConsumption.Cancel(userId);

        var correctionEntry = GasConsumptionEntity.CreateCorrection(originalConsumption, userId);

        var deliveriesToUpdate = new List<GasDeliveryEntity>();
        var correctionSources = new List<GasConsumptionSourceEntity>();

        foreach (var source in originalConsumption.ConsumptionSources)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(source.GasDeliveryId, cancellationToken);
            if (delivery != null)
            {
                delivery.AddUsedQuantity(-source.ConsumedQuantity);
                deliveriesToUpdate.Add(delivery);
            }

            correctionSources.Add(new GasConsumptionSourceEntity
            {
                GasConsumptionId = correctionEntry.Id,
                GasDeliveryId = source.GasDeliveryId,
                ConsumedQuantity = source.ConsumedQuantity
            });
        }

        await _consumptionRepository.UpdateAsync(originalConsumption, cancellationToken);
        await _consumptionRepository.AddAsync(correctionEntry, cancellationToken);
        await _consumptionSourceRepository.AddRangeAsync(correctionSources, cancellationToken);

        if (deliveriesToUpdate.Count != 0)
        {
            await _deliveryRepository.UpdateRangeAsync(deliveriesToUpdate, cancellationToken);
        }

        return BaseResponse.EmptyResponse;
    }
}