using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;

namespace FarmsManager.Application.Specifications.Sales;

public sealed class SaleDuplicateCheckSpec : BaseSpecification<SaleEntity>
{
    public SaleDuplicateCheckSpec(Guid cycleId, Guid farmId, Guid henhouseId, DateOnly saleDate, int quantity)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t =>
            t.CycleId == cycleId &&
            t.FarmId == farmId &&
            t.HenhouseId == henhouseId &&
            t.SaleDate == saleDate &&
            t.Quantity == quantity);
    }
}