using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;

namespace FarmsManager.Application.Specifications.FallenStocks;

public sealed class FallenStockPickupsByFarmAndCycleSpec : BaseSpecification<FallenStockPickupEntity>
{
    public FallenStockPickupsByFarmAndCycleSpec(Guid farmId, Guid cycleId, bool? showDeleted, bool isAdmin)
    {
        EnsureExists(showDeleted, isAdmin);
        Query.Where(t => t.FarmId == farmId);
        Query.Where(t => t.CycleId == cycleId);
    }
}