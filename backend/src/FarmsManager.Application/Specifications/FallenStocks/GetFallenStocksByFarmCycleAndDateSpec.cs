using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;

namespace FarmsManager.Application.Specifications.FallenStocks;

public sealed class GetFallenStocksByFarmCycleAndDateSpec : BaseSpecification<FallenStockEntity>
{
    public GetFallenStocksByFarmCycleAndDateSpec(Guid farmId, Guid cycleId, DateOnly date)
    {
        EnsureExists();
        Query.Where(t => t.FarmId == farmId && t.CycleId == cycleId && t.Date == date);
    }
}