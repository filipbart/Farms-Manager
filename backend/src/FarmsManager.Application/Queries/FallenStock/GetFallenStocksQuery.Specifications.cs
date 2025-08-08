using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;

namespace FarmsManager.Application.Queries.FallenStock;

public sealed class FallenStockByFarmAndCycleSpec : BaseSpecification<FallenStockEntity>
{
    public FallenStockByFarmAndCycleSpec(Guid farmId, Guid cycleId)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.FarmId == farmId);
        Query.Where(t => t.CycleId == cycleId);
    }
}

public sealed class GetSalesByFarmAndCycleSpec : BaseSpecification<SaleEntity>
{
    public GetSalesByFarmAndCycleSpec(Guid farmId, Guid cycleId)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.FarmId == farmId);
        Query.Where(t => t.CycleId == cycleId);
    }
}

public sealed class GetInsertionsByFarmAndCycleSpec : BaseSpecification<InsertionEntity>
{
    public GetInsertionsByFarmAndCycleSpec(Guid farmId, Guid cycleId)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.FarmId == farmId);
        Query.Where(t => t.CycleId == cycleId);
    }
}