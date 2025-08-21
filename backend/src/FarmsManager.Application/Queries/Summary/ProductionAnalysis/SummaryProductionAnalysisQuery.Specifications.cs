using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;

namespace FarmsManager.Application.Queries.Summary.ProductionAnalysis;

public sealed class GasConsumptionsByFarmsSpec : BaseSpecification<GasConsumptionEntity>
{
    public GasConsumptionsByFarmsSpec(List<Guid> farmIds)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.CancelledAtUtc.HasValue == false);
        Query.Where(t => farmIds.Contains(t.FarmId));
    }
}

public sealed class EmployeePayslipsByFarmsSpec : BaseSpecification<EmployeePayslipEntity>
{
    public EmployeePayslipsByFarmsSpec(List<Guid> farmIds)
    {
        EnsureExists();
        DisableTracking();
        Query.Include(t => t.Employee);
        Query.Where(t => farmIds.Contains(t.FarmId));
        Query.Where(t => t.Employee.Status == EmployeeStatus.Active);
        Query.Where(t => t.Employee.DateDeletedUtc.HasValue == false);
    }
}

public sealed class FeedsDeliveriesByHenhousesSpec : BaseSpecification<FeedInvoiceEntity>
{
    public FeedsDeliveriesByHenhousesSpec(List<Guid> henhousesIds)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => henhousesIds.Contains(t.HenhouseId));
    }
}

public sealed class ProductionDataRemainingFeedByHenhousesSpec : BaseSpecification<ProductionDataRemainingFeedEntity>
{
    public ProductionDataRemainingFeedByHenhousesSpec(List<Guid> henhousesIds)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => henhousesIds.Contains(t.HenhouseId));
    }
}

public sealed class ProductionDataTransferFeedByHenhousesSpec : BaseSpecification<ProductionDataTransferFeedEntity>
{
    public ProductionDataTransferFeedByHenhousesSpec(List<Guid> henhousesIds)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => henhousesIds.Contains(t.FromHenhouseId) || henhousesIds.Contains(t.ToHenhouseId));
    }
}