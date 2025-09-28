
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Queries.FallenStock;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public class
    GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, BaseResponse<GetDashboardStatsQueryResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IGasConsumptionRepository _gasConsumptionRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IInsertionRepository _insertionRepository;
    private readonly IProductionDataFailureRepository _productionDataFailureRepository;


    public GetDashboardStatsQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver,
        IFarmRepository farmRepository, ISaleRepository saleRepository, ICycleRepository cycleRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        IGasConsumptionRepository gasConsumptionRepository,
        IExpenseProductionRepository expenseProductionRepository, IInsertionRepository insertionRepository,
        IProductionDataFailureRepository productionDataFailureRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _saleRepository = saleRepository;
        _cycleRepository = cycleRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _gasConsumptionRepository = gasConsumptionRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _insertionRepository = insertionRepository;
        _productionDataFailureRepository = productionDataFailureRepository;
    }

    public async Task<BaseResponse<GetDashboardStatsQueryResponse>> Handle(GetDashboardStatsQuery request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new GetDashboardStatsQueryResponse());
        }

        var accessibleFarmIds = user.AccessibleFarmIds;

        var filteredFarmIds = request.Filters.FarmId.HasValue
            ? accessibleFarmIds != null && !accessibleFarmIds.Contains(request.Filters.FarmId.Value)
                ? throw DomainException.Forbidden()
                : new List<Guid> { request.Filters.FarmId.Value }
            : accessibleFarmIds;

        var farms = await _farmRepository.ListAsync(new GetAllFarmsSpec(filteredFarmIds), ct);
        if (farms.Count == 0)
        {
            return BaseResponse.CreateResponse(new GetDashboardStatsQueryResponse());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        // 1. POBIERZ DANE DLA GŁÓWNYCH STATYSTYK (Przychody, Koszty, VAT)
        var cycleIdsForFilter = await GetFilteredCycleIds(request, farms, ct);
        var filteredSales = await _saleRepository.ListAsync(
            new GetSalesForDashboardSpec(farmIds, cycleIdsForFilter, request.Filters.DateSince, request.Filters.DateTo),
            ct);
        var filteredFeedInvoices = await _feedInvoiceRepository.ListAsync(
            new GetFeedsInvoicesForDashboardSpec(farmIds, cycleIdsForFilter, request.Filters.DateSince,
                request.Filters.DateTo), ct);
        var filteredExpenses = await _expenseProductionRepository.ListAsync(
            new GetProductionExpensesForDashboardSpec(farmIds, cycleIdsForFilter, request.Filters.DateSince,
                request.Filters.DateTo), ct);

        decimal gasCostForStats;
        IReadOnlyList<ExpenseProductionEntity> otherExpenses;

        if (string.Equals(request.Filters.DateCategory, "cycle", StringComparison.OrdinalIgnoreCase))
        {
            gasCostForStats = await GetGasCostFromConsumptionsAsync(farmIds, cycleIdsForFilter, ct);
            otherExpenses = filteredExpenses;
        }
        else
        {
            var (gasExpenses, otherProdExpenses) = SplitGasExpenses(filteredExpenses);
            gasCostForStats = gasExpenses.Sum(e => e.SubTotal);
            otherExpenses = otherProdExpenses;
        }

        // 2. WYZNACZ I POBIERZ DANE SPECJALNIE DLA WSKAŹNIKÓW KPI (Dochód na kg/m²)
        decimal incomePerKg, incomePerSqm;

        switch (request.Filters.DateCategory?.ToLower())
        {
            case "year":
                var year = request.Filters.DateSince!.Value.Year;
                var yearStart = new DateOnly(year, 1, 1);
                var yearEnd = new DateOnly(year, 12, 31);

                var salesForYear =
                    await _saleRepository.ListAsync(new GetSalesForDashboardSpec(farmIds, null, yearStart, yearEnd),
                        ct);
                var feedsForYear =
                    await _feedInvoiceRepository.ListAsync(
                        new GetFeedsInvoicesForDashboardSpec(farmIds, null, yearStart, yearEnd), ct);
                var expensesForYear =
                    await _expenseProductionRepository.ListAsync(
                        new GetProductionExpensesForDashboardSpec(farmIds, null, yearStart, yearEnd), ct);
                var (gasExpensesForYear, otherExpensesForYear) = SplitGasExpenses(expensesForYear);
                var gasCostForYear = gasExpensesForYear.Sum(e => e.SubTotal);
                (incomePerKg, incomePerSqm) =
                    CalculateIncomeKpis(salesForYear, feedsForYear, otherExpensesForYear, gasCostForYear, farms);
                break;

            case "month":
                var activeCycleIdsForMonth = farms.Where(f => f.ActiveCycleId.HasValue).Select(f => f.ActiveCycleId.Value)
                    .ToList();
                if (activeCycleIdsForMonth.Any())
                {
                    var salesForActive =
                        await _saleRepository.ListAsync(new GetSalesForDashboardSpec(farmIds, activeCycleIdsForMonth), ct);
                    var feedsForActive =
                        await _feedInvoiceRepository.ListAsync(
                            new GetFeedsInvoicesForDashboardSpec(farmIds, activeCycleIdsForMonth), ct);
                    var expensesForActive =
                        await _expenseProductionRepository.ListAsync(
                            new GetProductionExpensesForDashboardSpec(farmIds, activeCycleIdsForMonth), ct);
                    var (gasExpensesForActive, otherExpensesForActive) = SplitGasExpenses(expensesForActive);
                    var gasCostForActive = gasExpensesForActive.Sum(e => e.SubTotal);
                    (incomePerKg, incomePerSqm) = CalculateIncomeKpis(salesForActive, feedsForActive, otherExpensesForActive,
                        gasCostForActive, farms);
                }
                else
                {
                    (incomePerKg, incomePerSqm) = (0, 0);
                }

                break;

            default:
                (incomePerKg, incomePerSqm) = CalculateIncomeKpis(filteredSales, filteredFeedInvoices, otherExpenses,
                    gasCostForStats, farms);
                break;
        }

        // 3. POBIERZ DANE DLA STATUSU KURNIKÓW (tylko dla aktywnych cykli)
        var activeCycleIds = farms.Where(f => f.ActiveCycleId.HasValue).Select(f => f.ActiveCycleId!.Value).ToList();
        var insertionsForStatus = activeCycleIds.Any()
            ? await _insertionRepository.ListAsync(new InsertionsForFarmsSpec(farmIds, activeCycleIds), ct)
            : new List<InsertionEntity>();
        var salesForStatus = activeCycleIds.Any()
            ? await _saleRepository.ListAsync(new GetSalesForDashboardSpec(farmIds, activeCycleIds), ct)
            : new List<SaleEntity>();
        var failuresForStatus = activeCycleIds.Any()
            ? await _productionDataFailureRepository.ListAsync(new ProductionDataFailuresForFarmsSpec(farmIds, activeCycleIds), ct)
            : new List<ProductionDataFailureEntity>();

        // 4. Budowanie komponentów odpowiedzi
        var stats = BuildDashboardStats(filteredSales, filteredFeedInvoices, otherExpenses, gasCostForStats,
            incomePerKg, incomePerSqm);
        var chickenHousesStatus = BuildChickenHousesStatus(farms, insertionsForStatus, salesForStatus, failuresForStatus);

        var response = new GetDashboardStatsQueryResponse
        {
            Stats = stats,
            ChickenHousesStatus = chickenHousesStatus,
        };

        return BaseResponse.CreateResponse(response);
    }

    private async Task<List<Guid>> GetFilteredCycleIds(GetDashboardStatsQuery request, List<FarmEntity> farms,
        CancellationToken ct)
    {
        var cycleIds = new List<Guid>();
        if (request.Filters.CycleDict == null) return cycleIds;

        foreach (var farm in farms)
        {
            var cycle = await _cycleRepository.FirstOrDefaultAsync(
                new GetCycleByYearIdentifierAndFarmSpec(farm.Id, request.Filters.CycleDict.Year,
                    request.Filters.CycleDict.Identifier), ct);
            cycleIds.Add(cycle?.Id ?? Guid.Empty);
        }

        return cycleIds;
    }

    private async Task<decimal> GetGasCostFromConsumptionsAsync(List<Guid> farmIds, List<Guid> cycleIds,
        CancellationToken ct)
    {
        if (cycleIds == null || !cycleIds.Any()) return 0;

        var gasConsumptions =
            await _gasConsumptionRepository.ListAsync(new GasConsumptionsForDashboardSpec(farmIds, cycleIds), ct);
        return gasConsumptions.Sum(t => t.Cost);
    }

    private static (IReadOnlyList<ExpenseProductionEntity> gasExpenses, IReadOnlyList<ExpenseProductionEntity>
        otherExpenses) SplitGasExpenses(IReadOnlyList<ExpenseProductionEntity> allExpenses)
    {
        var gasExpenses = allExpenses.Where(e =>
            string.Equals(e.ExpenseContractor?.ExpenseType?.Name, "Gaz", StringComparison.OrdinalIgnoreCase)).ToList();
        var otherExpenses = allExpenses.Except(gasExpenses).ToList();
        return (gasExpenses, otherExpenses);
    }

    private static (decimal incomePerKg, decimal incomePerSqm) CalculateIncomeKpis(IReadOnlyList<SaleEntity> sales,
        IReadOnlyList<FeedInvoiceEntity> feedInvoices,
        IReadOnlyList<ExpenseProductionEntity> expenses, decimal gasCost, IReadOnlyList<FarmEntity> farms)
    {
        var feedCosts = feedInvoices.Sum(t => t.SubTotal);
        var expensesCost = expenses.Sum(t => t.SubTotal);
        var totalRevenue = sales.Sum(s => (s.Weight - s.DeadWeight - s.ConfiscatedWeight) * s.PriceWithExtras);
        var sumExpenses = feedCosts + gasCost + expensesCost;

        var totalWeight = sales.Sum(t => t.Weight);
        var totalArea = farms.Sum(f => f.Henhouses?.Sum(h => h.Area) ?? 0);

        var incomePerKg = totalWeight > 0 ? (totalRevenue - sumExpenses) / totalWeight : 0;
        var incomePerSqm = totalArea > 0 ? (totalRevenue - sumExpenses) / totalArea : 0;

        return (incomePerKg, incomePerSqm);
    }

    private static DashboardStats BuildDashboardStats(IReadOnlyList<SaleEntity> sales,
        IReadOnlyList<FeedInvoiceEntity> feedInvoices, IReadOnlyList<ExpenseProductionEntity> otherExpenses,
        decimal gasCost,
        decimal incomePerKg, decimal incomePerSqm)
    {
        var feedCosts = feedInvoices.Sum(t => t.SubTotal);
        var expensesCost = otherExpenses.Sum(t => t.SubTotal);
        var totalRevenue = sales.Sum(s => (s.Weight - s.DeadWeight - s.ConfiscatedWeight) * s.PriceWithExtras);
        var sumExpenses = feedCosts + gasCost + expensesCost;

        var feedVat = feedInvoices.Sum(t => t.VatAmount);
        var expensesVat = otherExpenses.Sum(e => e.VatAmount);
        var gasVat = gasCost * 0.23m; // VAT od gazu jest teraz zawsze liczony od `gasCost`
        var sumVat = feedVat + gasVat + expensesVat;

        var totalFeedQuantity = feedInvoices.Sum(t => t.Quantity);

        return new DashboardStats
        {
            Revenue = totalRevenue,
            Expenses = sumExpenses,
            VatFromExpenses = sumVat,
            IncomePerKg = incomePerKg,
            IncomePerSqm = incomePerSqm,
            AvgFeedPrice = totalFeedQuantity > 0 ? Math.Round(feedCosts / totalFeedQuantity, 2) : 0
        };
    }

    private static DashboardChickenHousesStatus BuildChickenHousesStatus(
        IReadOnlyList<FarmEntity> farms,
        IReadOnlyList<InsertionEntity> allInsertions,
        IReadOnlyList<SaleEntity> allSales,
        IReadOnlyList<ProductionDataFailureEntity> allFailures)
    {
        var farmStatuses = new List<DashboardFarmStatus>();

        var insertionsLookup = allInsertions.ToLookup(i => i.HenhouseId);
        var salesLookup = allSales.ToLookup(s => s.HenhouseId);
        var failuresLookup = allFailures.ToLookup(f => f.HenhouseId);

        foreach (var farm in farms)
        {
            var farmStatus = new DashboardFarmStatus { Name = farm.Name };
            var activeCycleId = farm.ActiveCycleId;

            foreach (var henhouse in farm.Henhouses.Where(h => !h.DateDeletedUtc.HasValue).OrderBy(h => h.Name))
            {
                var chickenCount = 0;
                if (activeCycleId.HasValue)
                {
                    var henhouseInsertions = insertionsLookup[henhouse.Id].Where(i => i.CycleId == activeCycleId);
                    var henhouseSales = salesLookup[henhouse.Id].Where(s => s.CycleId == activeCycleId);
                    var henhouseFailures = failuresLookup[henhouse.Id].Where(f => f.CycleId == activeCycleId);

                    var insertedCount = henhouseInsertions.Sum(i => i.Quantity);
                    var lossesCount = henhouseFailures.Sum(f => f.DeadCount + f.DefectiveCount);
                    var soldCount = henhouseSales.Sum(s => s.Quantity);

                    chickenCount = insertedCount - soldCount - lossesCount;

                    if (insertedCount > 0 && chickenCount <= insertedCount * 0.04m)
                    {
                        chickenCount = 0;
                    }
                }

                farmStatus.Henhouses.Add(new DashboardHenhouseStatus
                {
                    Name = henhouse.Name,
                    ChickenCount = chickenCount < 0 ? 0 : chickenCount
                });
            }

            farmStatuses.Add(farmStatus);
        }

        return new DashboardChickenHousesStatus { Farms = farmStatuses };
    }
}
