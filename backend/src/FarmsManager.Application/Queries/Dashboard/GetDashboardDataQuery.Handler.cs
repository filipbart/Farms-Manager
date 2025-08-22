using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Notifications;
using FarmsManager.Application.Queries.FallenStock;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Queries.User;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
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
    GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, BaseResponse<GetDashboardDataQueryResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IInsertionRepository _insertionRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly IGasConsumptionRepository _gasConsumptionRepository;
    private readonly IEmployeeReminderRepository _employeeReminderRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IProductionDataFailureRepository _productionDataFailureRepository;

    public GetDashboardDataQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver,
        IFarmRepository farmRepository, ISaleRepository saleRepository, ICycleRepository cycleRepository,
        IEmployeeRepository employeeRepository, IInsertionRepository insertionRepository,
        IFeedInvoiceRepository feedInvoiceRepository, IGasDeliveryRepository gasDeliveryRepository,
        ISaleInvoiceRepository saleInvoiceRepository, IGasConsumptionRepository gasConsumptionRepository,
        IEmployeeReminderRepository employeeReminderRepository,
        IExpenseProductionRepository expenseProductionRepository,
        IProductionDataFailureRepository productionDataFailureRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _saleRepository = saleRepository;
        _cycleRepository = cycleRepository;
        _employeeRepository = employeeRepository;
        _insertionRepository = insertionRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
        _gasConsumptionRepository = gasConsumptionRepository;
        _employeeReminderRepository = employeeReminderRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _productionDataFailureRepository = productionDataFailureRepository;
    }

    public async Task<BaseResponse<GetDashboardDataQueryResponse>> Handle(GetDashboardDataQuery request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        var accessibleFarmIds = user.IsAdmin ? null : user.Farms?.Select(t => t.FarmId).ToList();

        var filteredFarmIds = request.Filters.FarmId.HasValue
            ? accessibleFarmIds != null && !accessibleFarmIds.Contains(request.Filters.FarmId.Value)
                ? throw DomainException.Forbidden()
                : new List<Guid> { request.Filters.FarmId.Value }
            : accessibleFarmIds;

        var farms = await _farmRepository.ListAsync(new GetAllFarmsSpec(filteredFarmIds), ct);
        if (farms.Count == 0)
        {
            return BaseResponse.CreateResponse(new GetDashboardDataQueryResponse());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        // ########## GŁÓWNA LOGIKA ##########

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
        var gasCostForStats = await GetGasCostAsync(farmIds, cycleIdsForFilter, request.Filters.DateSince,
            request.Filters.DateTo, ct);

        // 2. WYZNACZ I POBIERZ DANE SPECJALNIE DLA WSKAŹNIKÓW KPI (Dochód na kg/m²)
        decimal incomePerKg, incomePerSqm;

        switch (request.Filters.DateCategory?.ToLower())
        {
            case "year":
                // Dla roku: pobierz wszystkie dane z całego roku
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
                var gasCostForYear = await GetGasCostAsync(farmIds, null, yearStart, yearEnd, ct);
                (incomePerKg, incomePerSqm) =
                    CalculateIncomeKpis(salesForYear, feedsForYear, expensesForYear, gasCostForYear, farms);
                break;

            case "month":
                // Dla miesiąca: pobierz dane tylko dla aktywnych cykli
                var activeCycleIds = farms.Where(f => f.ActiveCycleId.HasValue).Select(f => f.ActiveCycleId.Value)
                    .ToList();
                if (activeCycleIds.Count != 0)
                {
                    var salesForActive =
                        await _saleRepository.ListAsync(new GetSalesForDashboardSpec(farmIds, activeCycleIds), ct);
                    var feedsForActive =
                        await _feedInvoiceRepository.ListAsync(
                            new GetFeedsInvoicesForDashboardSpec(farmIds, activeCycleIds), ct);
                    var expensesForActive =
                        await _expenseProductionRepository.ListAsync(
                            new GetProductionExpensesForDashboardSpec(farmIds, activeCycleIds), ct);
                    var gasCostForActive = await GetGasCostAsync(farmIds, activeCycleIds, null, null, ct);
                    (incomePerKg, incomePerSqm) = CalculateIncomeKpis(salesForActive, feedsForActive, expensesForActive,
                        gasCostForActive, farms);
                }
                else
                {
                    (incomePerKg, incomePerSqm) = (0, 0);
                }

                break;

            case "cycle":
            case "range":
            default:
                // Dla cyklu i zakresu dat: użyj tych samych danych co dla głównych statystyk
                (incomePerKg, incomePerSqm) = CalculateIncomeKpis(filteredSales, filteredFeedInvoices, filteredExpenses,
                    gasCostForStats, farms);
                break;
        }

        // 3. POBIERZ DANE DLA WYKRESÓW I POZOSTAŁYCH KOMPONENTÓW (zawsze historyczne)
        var historicalSales = await _saleRepository.ListAsync(new GetSalesForDashboardSpec(farmIds), ct);
        var historicalFeedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetFeedsInvoicesForDashboardSpec(farmIds), ct);
        var historicalExpenses =
            await _expenseProductionRepository.ListAsync(new GetProductionExpensesForDashboardSpec(farmIds), ct);
        var allInsertions =
            await _insertionRepository.ListAsync(new InsertionsForFarmsSpec(farmIds),
                ct); // Potrzebne dla EWW i statusu kurników
        var allFailures =
            await _productionDataFailureRepository.ListAsync(new ProductionDataFailuresForFarmsSpec(farmIds),
                ct); // Dla EWW, strat i statusu
        var allGasConsumptions =
            await _gasConsumptionRepository.ListAsync(new GasConsumptionsForFarmsSpec(farmIds), ct);
        var gasCostForCharts = allGasConsumptions.Sum(g => g.Cost);

        // 4. Budowanie komponentów odpowiedzi
        var stats = BuildDashboardStats(filteredSales, filteredFeedInvoices, filteredExpenses, gasCostForStats, farms,
            incomePerKg, incomePerSqm);
        var chickenHousesStatus = BuildChickenHousesStatus(farms, allInsertions, historicalSales, allFailures);
        var fcrChart = BuildFcrChart(farms, historicalSales, historicalFeedInvoices);
        var gasConsumptionChart = BuildGasConsumptionChart(farms, allGasConsumptions);
        var ewwChart = BuildEwwChart(farms, allInsertions, historicalSales, historicalFeedInvoices, allFailures);
        var flockLossChart = BuildFlockLossChart(farms, allFailures, allInsertions);
        var expensesPieChart = BuildExpensesPieChart(historicalFeedInvoices, historicalExpenses, gasCostForCharts);
        var notifications = await BuildDashboardNotifications(farmIds, ct);

        // 4. Składanie odpowiedzi
        var response = new GetDashboardDataQueryResponse
        {
            Stats = stats,
            ChickenHousesStatus = chickenHousesStatus,
            FcrChart = fcrChart,
            GasConsumptionChart = gasConsumptionChart,
            EwwChart = ewwChart,
            FlockLossChart = flockLossChart,
            ExpensesPieChart = expensesPieChart,
            Notifications = notifications
        };

        return BaseResponse.CreateResponse(response);
    }

    private async Task<List<Guid>> GetFilteredCycleIds(GetDashboardDataQuery request, List<FarmEntity> farms,
        CancellationToken ct)
    {
        var cycleIds = new List<Guid>();
        if (request.Filters.CycleDict == null) return cycleIds;

        foreach (var farm in farms)
        {
            var cycle = await _cycleRepository.FirstOrDefaultAsync(
                new GetCycleByYearIdentifierAndFarmSpec(farm.Id, request.Filters.CycleDict.Year,
                    request.Filters.CycleDict.Identifier), ct);
            if (cycle != null) cycleIds.Add(cycle.Id);
        }

        return cycleIds;
    }

    private async Task<decimal> GetGasCostAsync(List<Guid> farmIds, List<Guid> cycleIds, DateOnly? dateSince,
        DateOnly? dateTo, CancellationToken ct)
    {
        if (cycleIds != null && cycleIds.Count != 0)
        {
            var gasConsumptions =
                await _gasConsumptionRepository.ListAsync(new GasConsumptionsForDashboardSpec(farmIds, cycleIds), ct);
            return gasConsumptions.Sum(t => t.Cost);
        }

        var gasDeliveries =
            await _gasDeliveryRepository.ListAsync(new GasDeliveriesForDashboardSpec(farmIds, dateSince, dateTo), ct);
        return gasDeliveries.Sum(d => d.UnitPrice * d.UsedQuantity);
    }

    // Funkcja pomocnicza do obliczania wskaźników KPI
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

    // Zaktualizowana sygnatura BuildDashboardStats
    private static DashboardStats BuildDashboardStats(IReadOnlyList<SaleEntity> sales,
        IReadOnlyList<FeedInvoiceEntity> feedInvoices,
        IReadOnlyList<ExpenseProductionEntity> expenses, decimal gasCost, IReadOnlyList<FarmEntity> farms,
        decimal incomePerKg, decimal incomePerSqm)
    {
        var feedCosts = feedInvoices.Sum(t => t.SubTotal);
        var expensesCost = expenses.Sum(t => t.SubTotal);
        var totalRevenue = sales.Sum(s => (s.Weight - s.DeadWeight - s.ConfiscatedWeight) * s.PriceWithExtras);
        var sumExpenses = feedCosts + gasCost + expensesCost;

        var feedVat = feedInvoices.Sum(t => t.VatAmount);
        var expensesVat = expenses.Sum(e => e.VatAmount);
        var gasVat = gasCost * 0.23m;
        var sumVat = feedVat + gasVat + expensesVat;

        var totalFeedQuantity = feedInvoices.Sum(t => t.Quantity);

        return new DashboardStats
        {
            Revenue = totalRevenue,
            Expenses = sumExpenses,
            VatFromExpenses = sumVat,
            IncomePerKg = incomePerKg, // Użyj przekazanej wartości
            IncomePerSqm = incomePerSqm, // Użyj przekazanej wartości
            AvgFeedPrice = totalFeedQuantity > 0 ? Math.Round(feedCosts / totalFeedQuantity, 2) : 0
        };
    }

    private static DashboardFcrChart BuildFcrChart(IReadOnlyList<FarmEntity> farms, IReadOnlyList<SaleEntity> allSales,
        IReadOnlyList<FeedInvoiceEntity> allFeeds)
    {
        var result = new DashboardFcrChart();
        var salesLookup = allSales.ToLookup(s => s.FarmId);
        var feedsLookup = allFeeds.ToLookup(f => f.FarmId);

        foreach (var farm in farms)
        {
            var farmSalesByCycle = salesLookup[farm.Id]
                .GroupBy(s => s.CycleId)
                .ToDictionary(g => g.Key, g => g.Sum(s => s.Weight));

            var farmFeedsByCycle = feedsLookup[farm.Id]
                .GroupBy(f => f.CycleId)
                .ToDictionary(g => g.Key, g => g.Sum(f => f.Quantity));

            var chartSeries = new ChartSeries { FarmId = farm.Id, FarmName = farm.Name };

            foreach (var (cycleId, totalWeight) in farmSalesByCycle)
            {
                if (totalWeight > 0 && farmFeedsByCycle.TryGetValue(cycleId, out var totalFeed) && totalFeed > 0)
                {
                    var cycle = allSales.First(s => s.CycleId == cycleId).Cycle;
                    chartSeries.Data.Add(new ChartDataPoint
                    {
                        X = $"{cycle.Identifier}/{cycle.Year}",
                        Y = totalFeed * 1000 / totalWeight
                    });
                }
            }

            if (chartSeries.Data.Count != 0) result.Series.Add(chartSeries);
        }

        return result;
    }

    private static DashboardGasConsumptionChart BuildGasConsumptionChart(IReadOnlyList<FarmEntity> farms,
        IReadOnlyList<GasConsumptionEntity> gasConsumptions)
    {
        var result = new DashboardGasConsumptionChart();
        var consumptionsLookup = gasConsumptions.ToLookup(c => c.FarmId);

        foreach (var farm in farms)
        {
            var totalArea = farm.Henhouses?.Where(h => !h.DateDeletedUtc.HasValue).Sum(h => h.Area) ?? 0;
            if (totalArea == 0) continue;

            var farmConsumptionsByCycle = consumptionsLookup[farm.Id]
                .GroupBy(c => c.CycleId)
                .Select(g => new { g.First().Cycle, TotalQuantity = g.Sum(c => c.QuantityConsumed) });

            var chartSeries = new ChartSeries { FarmId = farm.Id, FarmName = farm.Name };
            foreach (var consumption in farmConsumptionsByCycle)
            {
                chartSeries.Data.Add(new ChartDataPoint
                {
                    X = $"{consumption.Cycle.Identifier}/{consumption.Cycle.Year}",
                    Y = Math.Round(consumption.TotalQuantity / totalArea, 2)
                });
            }

            if (chartSeries.Data.Count != 0) result.Series.Add(chartSeries);
        }

        return result;
    }

    private static DashboardEwwChart BuildEwwChart(IReadOnlyList<FarmEntity> farms,
        IReadOnlyList<InsertionEntity> allInsertions, IReadOnlyList<SaleEntity> allSales,
        IReadOnlyList<FeedInvoiceEntity> allFeeds, IReadOnlyList<ProductionDataFailureEntity> allFailures)
    {
        var result = new DashboardEwwChart();
        var salesLookup = allSales.ToLookup(s => s.CycleId);
        var feedsLookup = allFeeds.ToLookup(f => f.CycleId);
        var failuresLookup = allFailures.ToLookup(f => f.CycleId);
        var farmsDict = farms.ToDictionary(f => f.Id);
        var seriesByFarm = new Dictionary<Guid, ChartSeries>();

        foreach (var insertion in allInsertions)
        {
            var cycleSales = salesLookup[insertion.CycleId].ToList();
            if (cycleSales.Count == 0) continue;

            var insertionQuantity = (decimal?)insertion.Quantity;
            if (insertionQuantity is null or 0) continue;

            var totalLosses = cycleSales.Sum(s => s.DeadCount + s.ConfiscatedCount) +
                              failuresLookup[insertion.CycleId].Sum(f => f.DeadCount + f.DefectiveCount);

            if (insertionQuantity == 0) continue;
            var survivalRatePct = (insertionQuantity.Value - totalLosses) * 100 / insertionQuantity.Value;

            var combinedSoldCount = (decimal)cycleSales.Sum(s => s.Quantity);
            if (combinedSoldCount == 0) continue;
            var combinedAvgWeight = cycleSales.Sum(s => s.Weight) / combinedSoldCount;

            var totalSettlementCount = cycleSales.Sum(s => s.Quantity - s.DeadCount - s.ConfiscatedCount);
            if (totalSettlementCount == 0) continue;
            var weightedAgeSum = cycleSales.Sum(s =>
                (s.Quantity - s.DeadCount - s.ConfiscatedCount) *
                (s.SaleDate.DayNumber - insertion.InsertionDate.DayNumber));
            var combinedAvgAgeInDays = (decimal)weightedAgeSum / totalSettlementCount;
            if (combinedAvgAgeInDays == 0) continue;

            var combinedSettlementWeight = cycleSales.Sum(s => s.Weight - s.DeadWeight - s.ConfiscatedWeight);
            if (combinedSettlementWeight is 0) continue;
            var fcrWithoutLosses =
                feedsLookup[insertion.CycleId].Sum(f => f.Quantity) * 1000 / combinedSettlementWeight;

            var denominator = combinedAvgAgeInDays * fcrWithoutLosses;
            if (denominator == 0) continue;

            var eww = survivalRatePct * combinedAvgWeight / denominator * 100;

            if (!seriesByFarm.TryGetValue(insertion.FarmId, out var chartSeries))
            {
                chartSeries = new ChartSeries
                {
                    FarmId = insertion.FarmId,
                    FarmName = farmsDict.TryGetValue(insertion.FarmId, out var farm) ? farm.Name : "Nieznana ferma"
                };
                seriesByFarm[insertion.FarmId] = chartSeries;
            }

            chartSeries.Data.Add(new ChartDataPoint
            {
                X = $"{insertion.Cycle.Identifier}/{insertion.Cycle.Year}",
                Y = eww
            });
        }

        result.Series = seriesByFarm.Values.ToList();
        result.Series.ForEach(s =>
            s.Data = s.Data.OrderBy(d => int.Parse(d.X.Split('/')[1]) * 100 + int.Parse(d.X.Split('/')[0])).ToList());

        return result;
    }

    private static DashboardFlockLossChart BuildFlockLossChart(IReadOnlyList<FarmEntity> farms,
        IReadOnlyList<ProductionDataFailureEntity> allFailures, IReadOnlyList<InsertionEntity> allInsertions)
    {
        var result = new DashboardFlockLossChart();
        var failuresLookup = allFailures.ToLookup(f => f.FarmId);
        var insertionsByCycleLookup = allInsertions.ToLookup(i => i.CycleId);

        foreach (var farm in farms)
        {
            var chartSeries = new ChartSeries { FarmId = farm.Id, FarmName = farm.Name };

            var farmFailuresByCycle = failuresLookup[farm.Id]
                .GroupBy(f => f.CycleId)
                .Select(g => new
                {
                    g.First().Cycle,
                    TotalLossCount = g.Sum(c => c.DeadCount + c.DefectiveCount)
                });

            foreach (var cycleFailures in farmFailuresByCycle)
            {
                var cycleInsertions = insertionsByCycleLookup[cycleFailures.Cycle.Id].ToList();
                var totalInserted = cycleInsertions.Sum(i => i.Quantity);

                if (totalInserted > 0)
                {
                    var lossPercentage = (decimal)cycleFailures.TotalLossCount * 100 / totalInserted;
                    chartSeries.Data.Add(new ChartDataPoint
                    {
                        X = $"{cycleFailures.Cycle.Identifier}/{cycleFailures.Cycle.Year}",
                        Y = Math.Round(lossPercentage, 2)
                    });
                }
            }

            if (chartSeries.Data.Count != 0) result.Series.Add(chartSeries);
        }

        return result;
    }

    private static DashboardExpensesPieChart BuildExpensesPieChart(
        IReadOnlyList<FeedInvoiceEntity> allFeedInvoices,
        IReadOnlyList<ExpenseProductionEntity> allExpenses,
        decimal totalGasCost)
    {
        var feedCost = allFeedInvoices.Sum(f => f.SubTotal);
        decimal chicksCost = 0;
        decimal vetCareCost = 0;
        decimal otherCosts = 0;

        foreach (var expense in allExpenses)
        {
            var expenseType = expense.ExpenseContractor?.ExpenseType?.Name ?? string.Empty;
            if (string.Equals(expenseType, "Zakup piskląt", StringComparison.OrdinalIgnoreCase))
                chicksCost += expense.SubTotal;
            else if (string.Equals(expenseType, "Usługa weterynaryjna", StringComparison.OrdinalIgnoreCase))
                vetCareCost += expense.SubTotal;
            else
                otherCosts += expense.SubTotal;
        }

        var totalExpenses = feedCost + totalGasCost + chicksCost + vetCareCost + otherCosts;

        var data = new List<ExpensesPieChartDataPoint>();

        if (totalExpenses == 0)
        {
            return new DashboardExpensesPieChart { Data = data };
        }

        if (feedCost > 0)
            data.Add(new ExpensesPieChartDataPoint
                { Id = "feed", Label = "Pasza", Value = Math.Round(feedCost / totalExpenses * 100, 2) });

        if (totalGasCost > 0)
            data.Add(new ExpensesPieChartDataPoint
                { Id = "gas", Label = "Gaz", Value = Math.Round(totalGasCost / totalExpenses * 100, 2) });

        if (chicksCost > 0)
            data.Add(new ExpensesPieChartDataPoint
                { Id = "chicks", Label = "Pisklęta", Value = Math.Round(chicksCost / totalExpenses * 100, 2) });

        if (vetCareCost > 0)
            data.Add(new ExpensesPieChartDataPoint
                { Id = "vet", Label = "Obsługa wet.", Value = Math.Round(vetCareCost / totalExpenses * 100, 2) });

        if (otherCosts > 0)
            data.Add(new ExpensesPieChartDataPoint
                { Id = "other", Label = "Pozostałe", Value = Math.Round(otherCosts / totalExpenses * 100, 2) });

        return new DashboardExpensesPieChart { Data = data.OrderByDescending(d => d.Value).ToList() };
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

    private record NotificationSource(
        DateOnly DueDate,
        NotificationType Type,
        object Entity,
        Guid SourceId,
        int SortPriority);

    private async Task<List<DashboardNotificationItem>> BuildDashboardNotifications(List<Guid> farmIds,
        CancellationToken ct)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        var sevenDaysFromNow = now.AddDays(7);
        const int daysForMediumPriority = 3;

        var allSources = new List<NotificationSource>();

        var salesInvoices =
            await _saleInvoiceRepository.ListAsync(new GetOverdueAndUpcomingSaleInvoicesSpec(sevenDaysFromNow, farmIds),
                ct);
        allSources.AddRange(salesInvoices.Select(inv =>
            new NotificationSource(inv.DueDate, NotificationType.SaleInvoice, inv, inv.Id, 2)));

        var feedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetOverdueAndUpcomingFeedInvoicesSpec(sevenDaysFromNow, farmIds),
                ct);
        allSources.AddRange(feedInvoices.Select(inv =>
            new NotificationSource(inv.DueDate, NotificationType.FeedInvoice, inv, inv.Id, 2)));

        var expiringContracts =
            await _employeeRepository.ListAsync(
                new GetOverdueAndUpcomingEmployeesContractSpec(sevenDaysFromNow, farmIds), ct);
        allSources.AddRange(expiringContracts.Select(emp =>
            new NotificationSource(emp.EndDate!.Value, NotificationType.EmployeeContract, emp, emp.Id, 1)));

        var employeeReminders =
            await _employeeReminderRepository.ListAsync(
                new GetOverdueAndUpcomingEmployeesRemindersSpec(now, sevenDaysFromNow, farmIds), ct);
        allSources.AddRange(employeeReminders.Select(rem =>
            new NotificationSource(rem.DueDate, NotificationType.EmployeeReminder, rem, rem.EmployeeId, 1)));

        var top5Notifications = allSources
            .OrderBy(s => s.SortPriority)
            .ThenBy(s => s.DueDate)
            .Take(5)
            .Select(source =>
            {
                NotificationPriority priority;
                if (source.DueDate <= now) priority = NotificationPriority.High;
                else if (source.DueDate <= now.AddDays(daysForMediumPriority)) priority = NotificationPriority.Medium;
                else priority = NotificationPriority.Low;

                var description = GenerateDescription(source, now);

                return new DashboardNotificationItem
                {
                    Description = description,
                    DueDate = source.DueDate,
                    Priority = priority,
                    Type = source.Type,
                    SourceId = source.SourceId
                };
            })
            .ToList();

        return top5Notifications;
    }

    private static string GenerateDescription(NotificationSource source, DateOnly now)
    {
        // Używamy formatowania z polskimi nazwami dni tygodnia
        var culture = new System.Globalization.CultureInfo("pl-PL");
        var dateString = source.DueDate.ToString("d MMMM yyyy", culture);
        string dayDifference;
        var days = source.DueDate.DayNumber - now.DayNumber;
        if (days < 0) dayDifference = $" (zaległe o {-days} dni)";
        else if (days == 0) dayDifference = " (dziś)";
        else if (days == 1) dayDifference = " (jutro)";
        else dayDifference = $" (za {days} dni)";

        return source.Type switch
        {
            NotificationType.SaleInvoice => $"Faktura sprzedaży: termin płatności {dateString}{dayDifference}",
            NotificationType.FeedInvoice => $"Faktura za paszę: termin płatności {dateString}{dayDifference}",
            NotificationType.EmployeeContract =>
                $"Koniec umowy dla {(source.Entity as EmployeeEntity)?.FullName}: {dateString}{dayDifference}",
            NotificationType.EmployeeReminder =>
                $"Przypomnienie '{(source.Entity as EmployeeReminderEntity)?.Title}': {dateString}{dayDifference}",
            _ => "Nieznane powiadomienie"
        };
    }
}