using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Enum;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Queries.Dashboard;

public sealed class GetSalesForDashboardSpec : BaseSpecification<SaleEntity>
{
    public GetSalesForDashboardSpec(List<Guid> farmIds = null, List<Guid> cycles = null, DateOnly? dateSince = null,
        DateOnly? dateTo = null)
    {
        EnsureExists();
        DisableTracking();
        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);

        if (farmIds is not null && farmIds.Count != 0)
        {
            Query.Where(t => farmIds.Contains(t.FarmId));
        }

        if (cycles is not null && cycles.Count != 0)
        {
            Query.Where(t => cycles.Contains(t.CycleId));
        }

        if (dateSince is not null)
        {
            Query.Where(t => t.SaleDate >= dateSince);
        }

        if (dateTo is not null)
        {
            Query.Where(t => t.SaleDate <= dateTo);
        }
    }
}

public sealed class GetFeedsInvoicesForDashboardSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetFeedsInvoicesForDashboardSpec(List<Guid> farmIds = null, List<Guid> cycles = null,
        DateOnly? dateSince = null,
        DateOnly? dateTo = null)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);

        if (farmIds is not null && farmIds.Count != 0)
        {
            Query.Where(t => farmIds.Contains(t.FarmId));
        }

        if (cycles is not null && cycles.Count != 0)
        {
            Query.Where(t => cycles.Contains(t.CycleId));
        }

        if (dateSince is not null)
        {
            Query.Where(t => t.InvoiceDate >= dateSince);
        }

        if (dateTo is not null)
        {
            Query.Where(t => t.InvoiceDate <= dateTo);
        }
    }
}

public sealed class GetProductionExpensesForDashboardSpec : BaseSpecification<ExpenseProductionEntity>
{
    public GetProductionExpensesForDashboardSpec(List<Guid> farmIds = null, List<Guid> cycles = null,
        DateOnly? dateSince = null, DateOnly? dateTo = null, bool? isGas = null)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);
        // Dołączamy powiązane encje, aby móc filtrować po nazwie typu wydatku
        Query.Include(t => t.ExpenseContractor)
            .ThenInclude(ec => ec.ExpenseType);

        if (farmIds is not null && farmIds.Count != 0)
        {
            Query.Where(t => farmIds.Contains(t.FarmId));
        }

        if (cycles is not null && cycles.Count != 0)
        {
            Query.Where(t => cycles.Contains(t.CycleId));
        }

        if (dateSince is not null)
        {
            Query.Where(t => t.InvoiceDate >= dateSince);
        }

        if (dateTo is not null)
        {
            Query.Where(t => t.InvoiceDate <= dateTo);
        }

        if (isGas.HasValue)
        {
            // Używamy ToLower() do porównania bez uwzględniania wielkości liter, co jest tłumaczone na odpowiednie zapytanie SQL
            const string gasTypeName = "gaz";
            if (isGas.Value)
            {
                Query.Where(e => EF.Functions.ILike(e.ExpenseContractor.ExpenseType.Name, gasTypeName));
            }
            else
            {
                Query.Where(e => !EF.Functions.ILike(e.ExpenseContractor.ExpenseType.Name, gasTypeName));
            }
        }
    }
}

public sealed class GasConsumptionsForDashboardSpec : BaseSpecification<GasConsumptionEntity>
{
    public GasConsumptionsForDashboardSpec(List<Guid> farmIds, List<Guid> cycles)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);

        Query.Where(gc => gc.Status == GasConsumptionStatus.Active && gc.CorrectionForId == null);
        Query.Where(t => t.CancelledAtUtc.HasValue == false);
        Query.Where(t => farmIds.Contains(t.FarmId));
        Query.Where(t => cycles.Contains(t.CycleId));
    }
}

public sealed class GasDeliveriesForDashboardSpec : BaseSpecification<GasDeliveryEntity>
{
    public GasDeliveriesForDashboardSpec(List<Guid> farmIds, DateOnly? dateSince, DateOnly? dateTo)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(t => t.Farm);

        Query.Where(t => farmIds.Contains(t.FarmId));
        if (dateSince.HasValue)
            Query.Where(t => t.InvoiceDate >= dateSince);

        if (dateTo.HasValue)
            Query.Where(t => t.InvoiceDate <= dateTo);
    }
}

public sealed class GasConsumptionsForFarmsSpec : BaseSpecification<GasConsumptionEntity>
{
    public GasConsumptionsForFarmsSpec(List<Guid> farmIds)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);

        Query.Where(gc => gc.Status == GasConsumptionStatus.Active && gc.CorrectionForId == null);
        Query.Where(t => t.CancelledAtUtc.HasValue == false);
        Query.Where(t => farmIds.Contains(t.FarmId));
    }
}

public sealed class InsertionsForFarmsSpec : BaseSpecification<InsertionEntity>
{
    public InsertionsForFarmsSpec(List<Guid> farmIds, List<Guid> cycleIds = null)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);

        Query.Where(t => farmIds.Contains(t.FarmId));

        if (cycleIds is not null && cycleIds.Any())
        {
            Query.Where(t => cycleIds.Contains(t.CycleId));
        }
    }
}

public class ProductionDataFailuresForFarmsSpec : BaseSpecification<ProductionDataFailureEntity>
{
    public ProductionDataFailuresForFarmsSpec(List<Guid> farmIds, List<Guid> cycleIds = null)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);

        Query.Where(f => farmIds.Contains(f.FarmId));

        if (cycleIds is not null && cycleIds.Any())
        {
            Query.Where(t => cycleIds.Contains(t.CycleId));
        }
    }
}

public sealed class GetOverdueAndUpcomingEmployeesRemindersSpec : BaseSpecification<EmployeeReminderEntity>
{
    public GetOverdueAndUpcomingEmployeesRemindersSpec(DateOnly today, DateOnly limitDate, List<Guid> accessibleFarmIds)
    {
        EnsureExists();

        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.Employee.FarmId));

        Query.Where(t => today >= t.DueDate.AddDays(-t.DaysToRemind))
            .Where(t => t.DueDate <= limitDate);
    }
}