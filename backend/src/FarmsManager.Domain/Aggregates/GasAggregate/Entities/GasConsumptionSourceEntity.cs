using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.GasAggregate.Entities;

public class GasConsumptionSourceEntity : Entity
{
    public Guid GasConsumptionId { get; init; } 
    public Guid GasDeliveryId { get; init; }    
    public decimal ConsumedQuantity { get; init; } 

    public virtual GasConsumptionEntity GasConsumption { get; init; }
    public virtual GasDeliveryEntity GasDelivery { get; init; }
}