using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Gas.Consumptions;

public record CalculateCostForConsumptionQuery : IRequest<BaseResponse<CalculateCostForConsumptionQueryResponse>>
{
    public Guid FarmId { get; init; }
    public decimal Quantity { get; init; }
}

public class CalculateCostForConsumptionQueryResponse
{
    public decimal Cost { get; set; }
}

public class CalculateCostForConsumptionQueryHandler : IRequestHandler<CalculateCostForConsumptionQuery,
    BaseResponse<CalculateCostForConsumptionQueryResponse>>
{
    private readonly IGasDeliveryRepository _gasDeliveryRepository;

    public CalculateCostForConsumptionQueryHandler(IGasDeliveryRepository gasDeliveryRepository)
    {
        _gasDeliveryRepository = gasDeliveryRepository;
    }

    public async Task<BaseResponse<CalculateCostForConsumptionQueryResponse>> Handle(
        CalculateCostForConsumptionQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAvailableGasDeliveriesForFarmSpec(request.FarmId);
        var availableDeliveries = await _gasDeliveryRepository.ListAsync(spec, cancellationToken);

        decimal totalCost = 0;
        var quantityToConsume = request.Quantity;

        foreach (var delivery in availableDeliveries)
        {
            if (quantityToConsume <= 0) break;

            var availableQuantityInDelivery = delivery.Quantity - delivery.UsedQuantity;
            var quantityToTake = Math.Min(quantityToConsume, availableQuantityInDelivery);

            totalCost += quantityToTake * delivery.UnitPrice;
            quantityToConsume -= quantityToTake;
        }

        if (quantityToConsume > 0)
        {
            throw new Exception("Niewystarczająca ilość gazu w dostępnych dostawach, aby pokryć podane zużycie.");
        }

        return BaseResponse.CreateResponse(new CalculateCostForConsumptionQueryResponse
        {
            Cost = Math.Round(totalCost, 2)
        });
    }
}

public sealed class GetAvailableGasDeliveriesForFarmSpec : BaseSpecification<GasDeliveryEntity>
{
    public GetAvailableGasDeliveriesForFarmSpec(Guid farmId)
    {
        EnsureExists();
        Query
            .Where(d => d.FarmId == farmId && d.UsedQuantity < d.Quantity)
            .OrderBy(d => d.InvoiceDate);
    }
}