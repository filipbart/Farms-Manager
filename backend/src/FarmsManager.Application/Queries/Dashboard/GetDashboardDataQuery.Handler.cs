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


    public async Task<BaseResponse<GetDashboardDataQueryResponse>> Handle(GetDashboardDataQuery request,
        CancellationToken ct)
    {
        // 1. Ustalenie uprawnień i filtrów
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        var accessibleFarmIds = user.IsAdmin ? null : user.Farms?.Select(t => t.FarmId).ToList();

        var filteredFarmIds = request.Filters.FarmId.HasValue
            ? accessibleFarmIds is not null && !accessibleFarmIds.Contains(request.Filters.FarmId.Value)
                ? throw DomainException.Forbidden()
                : new List<Guid> { request.Filters.FarmId.Value }
            : accessibleFarmIds;

        // 2. JEDNORAZOWE pobranie wszystkich niezbędnych danych
        var farms = await _farmRepository.ListAsync(new GetAllFarmsSpec(filteredFarmIds), ct);
        if (farms.Count == 0)
        {
            return BaseResponse.CreateResponse(new GetDashboardDataQueryResponse());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        var cycles = await GetFilteredCycleIds(request, farms, ct);

        // Pobieramy dane z repozytoriów
        var allSales = await _saleRepository.ListAsync(
            new GetSalesForDashboardSpec(farmIds, cycles, request.Filters.DateSince, request.Filters.DateTo), ct);
        var allFeedInvoices = await _feedInvoiceRepository.ListAsync(
            new GetFeedsInvoicesForDashboardSpec(farmIds, cycles, request.Filters.DateSince, request.Filters.DateTo),
            ct);
        var allExpenses = await _expenseProductionRepository.ListAsync(
            new GetProductionExpensesForDashboardSpec(farmIds, cycles, request.Filters.DateSince,
                request.Filters.DateTo), ct);
        var allInsertions =
            await _insertionRepository.ListAsync(new InsertionsForFarmsSpec(farmIds),
                ct); // Potrzebne dla EWW i statusu kurników
        var allFailures =
            await _productionDataFailureRepository.ListAsync(new ProductionDataFailuresForFarmsSpec(farmIds),
                ct); // Dla EWW, strat i statusu

        decimal gasCost = 0;
        if (request.Filters.CycleDict is not null)
        {
            var gasConsumptions =
                await _gasConsumptionRepository.ListAsync(new GasConsumptionsForDashboardSpec(farmIds, cycles), ct);
            gasCost = gasConsumptions.Sum(t => t.Cost);
        }
        else
        {
            var gasDeliveries = await _gasDeliveryRepository.ListAsync(
                new GasDeliveriesForDashboardSpec(farmIds, request.Filters.DateSince, request.Filters.DateTo), ct);
            gasCost = gasDeliveries.Sum(d => d.UnitPrice * d.UsedQuantity);
        }

        // 3. RÓWNOLEGŁE budowanie komponentów dashboardu
        var statsTask = Task.Run(() =>
            BuildDashboardStats(request, farmIds, cycles, allSales, allFeedInvoices, allExpenses, farms, ct), ct);
        var chickenHousesStatusTask =
            Task.Run(() => BuildChickenHousesStatus(farms, allInsertions, allSales, allFailures), ct);
        var fcrChartTask = Task.Run(() => BuildFcrChart(farms, allSales, allFeedInvoices), ct);
        var gasConsumptionChartTask = Task.Run(() => BuildGasConsumptionChart(farms, farmIds, ct), ct);
        var ewwChartTask = Task.Run(() => BuildEwwChart(farms, allInsertions, allSales, allFeedInvoices, allFailures),
            ct);
        var flockLossChartTask = Task.Run(() => BuildFlockLossChart(farms, allFailures), ct);
        var expensesPieChartTask = Task.Run(() => BuildExpensesPieChart(allFeedInvoices, allExpenses, gasCost), ct);

        await Task.WhenAll(
            statsTask, chickenHousesStatusTask, fcrChartTask,
            gasConsumptionChartTask, ewwChartTask, flockLossChartTask, expensesPieChartTask
        );

        // 4. Złożenie finalnej odpowiedzi
        var response = new GetDashboardDataQueryResponse
        {
            Stats = await statsTask,
            ChickenHousesStatus = chickenHousesStatusTask.Result,
            FcrChart = fcrChartTask.Result,
            GasConsumptionChart = await gasConsumptionChartTask,
            EwwChart = ewwChartTask.Result,
            FlockLossChart = flockLossChartTask.Result,
            ExpensesPieChart = expensesPieChartTask.Result
        };

        return BaseResponse.CreateResponse(response);
    }

    private async Task<List<Guid>> GetFilteredCycleIds(GetDashboardDataQuery request, List<FarmEntity> farms,
        CancellationToken ct)
    {
        if (request.Filters.CycleDict == null)
        {
            return null;
        }

        var cycleIds = new List<Guid>();
        foreach (var farm in farms)
        {
            var cycle = await _cycleRepository.FirstOrDefaultAsync(
                new GetCycleByYearIdentifierAndFarmSpec(farm.Id, request.Filters.CycleDict.Year,
                    request.Filters.CycleDict.Identifier), ct);
            if (cycle != null)
            {
                cycleIds.Add(cycle.Id);
            }
        }

        return cycleIds;
    }

    private async Task<DashboardStats> BuildDashboardStats(GetDashboardDataQuery request, List<Guid> farmIds,
        List<Guid> cycles,
        IReadOnlyList<SaleEntity> sales, IReadOnlyList<FeedInvoiceEntity> feedInvoices,
        IReadOnlyList<ExpenseProductionEntity> expenses, IReadOnlyList<FarmEntity> farms, CancellationToken ct)
    {
        // Koszty paszy i wydatków
        var feedCosts = feedInvoices.Sum(t => t.SubTotal);
        var feedVat = feedInvoices.Sum(t => t.VatAmount);
        var expensesCost = expenses.Sum(t => t.SubTotal);
        var expensesVat = expenses.Sum(t => t.VatAmount);

        // Koszty gazu (logika warunkowa)
        decimal gasCost;
        if (request.Filters.CycleDict is not null)
        {
            var gasConsumptions =
                await _gasConsumptionRepository.ListAsync(new GasConsumptionsForDashboardSpec(farmIds, cycles), ct);
            gasCost = gasConsumptions.Sum(t => t.Cost);
            // Założenie stałej stawki VAT
        }
        else
        {
            var gasDeliveries = await _gasDeliveryRepository.ListAsync(
                new GasDeliveriesForDashboardSpec(farmIds, request.Filters.DateSince, request.Filters.DateTo), ct);
            gasCost = gasDeliveries.Sum(d => d.UnitPrice * d.UsedQuantity);
        }

        var gasVat = gasCost * 0.23m; // Założenie stałej stawki VAT

        // Przychody
        var totalRevenue = sales.Sum(s => (s.Weight - s.DeadWeight - s.ConfiscatedWeight) * s.PriceWithExtras);

        var sumExpenses = feedCosts + gasCost + expensesCost;
        var sumVat = feedVat + gasVat + expensesVat;

        var totalArea = farms.Sum(f => f.Henhouses?.Sum(h => h.Area) ?? 0);
        var totalWeight = sales.Sum(t => t.Weight);
        var totalFeedQuantity = feedInvoices.Sum(t => t.Quantity);

        return new DashboardStats
        {
            Revenue = totalRevenue,
            Expenses = sumExpenses,
            VatFromExpenses = sumVat,
            IncomePerKg = totalWeight > 0 ? (totalRevenue - sumExpenses) / totalWeight : 0,
            IncomePerSqm = totalArea > 0 ? (totalRevenue - sumExpenses) / totalArea : 0,
            AvgFeedPrice = totalFeedQuantity > 0 ? feedCosts / totalFeedQuantity : 0
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

    private async Task<DashboardGasConsumptionChart> BuildGasConsumptionChart(IReadOnlyList<FarmEntity> farms,
        List<Guid> farmIds, CancellationToken ct)
    {
        var result = new DashboardGasConsumptionChart();
        var gasConsumptions = await _gasConsumptionRepository.ListAsync(new GasConsumptionsForFarmsSpec(farmIds), ct);
        var consumptionsLookup = gasConsumptions.ToLookup(c => c.FarmId);

        foreach (var farm in farms)
        {
            var farmConsumptionsByCycle = consumptionsLookup[farm.Id]
                .GroupBy(c => c.CycleId)
                .Select(g => new { g.First().Cycle, TotalQuantity = g.Sum(c => c.QuantityConsumed) });

            var chartSeries = new ChartSeries { FarmId = farm.Id, FarmName = farm.Name };
            foreach (var consumption in farmConsumptionsByCycle)
            {
                chartSeries.Data.Add(new ChartDataPoint
                {
                    X = $"{consumption.Cycle.Identifier}/{consumption.Cycle.Year}",
                    Y = consumption.TotalQuantity
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
                    FarmName = farmsDict.TryGetValue(insertion.FarmId, out var farm) ? farm.Name : "Nieznana ferma",
                    Data = []
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
        IReadOnlyList<ProductionDataFailureEntity> allFailures)
    {
        var result = new DashboardFlockLossChart();
        var failuresLookup = allFailures.ToLookup(f => f.FarmId);

        foreach (var farm in farms)
        {
            var farmFailuresByCycle = failuresLookup[farm.Id]
                .GroupBy(f => f.CycleId)
                .Select(g => new { g.First().Cycle, TotalCount = g.Sum(c => c.DeadCount + c.DefectiveCount) });

            var chartSeries = new ChartSeries { FarmId = farm.Id, FarmName = farm.Name };
            foreach (var failure in farmFailuresByCycle)
            {
                chartSeries.Data.Add(new ChartDataPoint
                {
                    X = $"{failure.Cycle.Identifier}/{failure.Cycle.Year}",
                    Y = failure.TotalCount
                });
            }

            if (chartSeries.Data.Count != 0) result.Series.Add(chartSeries);
        }

        return result;
    }

    private DashboardExpensesPieChart BuildExpensesPieChart(
        IReadOnlyList<FeedInvoiceEntity> allFeedInvoices,
        IReadOnlyList<ExpenseProductionEntity> allExpenses,
        decimal totalGasCost)
    {
        // 1. Koszt paszy - suma wartości netto z faktur
        var feedCost = allFeedInvoices.Sum(f => f.SubTotal);

        // 2. Kategoryzacja pozostałych wydatków
        decimal chicksCost = 0;
        decimal vetCareCost = 0;
        decimal otherCosts = 0;

        foreach (var expense in allExpenses)
        {
            var expenseType = expense.ExpenseContractor?.ExpenseType?.Name ?? string.Empty;

            if (string.Equals(expenseType, "Zakup piskląt", StringComparison.OrdinalIgnoreCase))
            {
                chicksCost += expense.SubTotal;
            }
            else if (string.Equals(expenseType, "Usługa weterynaryjna", StringComparison.OrdinalIgnoreCase))
            {
                vetCareCost += expense.SubTotal;
            }
            else
            {
                otherCosts += expense.SubTotal;
            }
        }

        var data = new List<ExpensesPieChartDataPoint>();

        // Dodajemy tylko te kategorie, których koszt jest większy od zera
        if (feedCost > 0)
            data.Add(new ExpensesPieChartDataPoint { Id = "feed", Label = "Pasza", Value = Math.Round(feedCost, 2) });

        if (totalGasCost > 0)
            data.Add(new ExpensesPieChartDataPoint { Id = "gas", Label = "Gaz", Value = Math.Round(totalGasCost, 2) });

        if (chicksCost > 0)
            data.Add(new ExpensesPieChartDataPoint
                { Id = "chicks", Label = "Pisklęta", Value = Math.Round(chicksCost, 2) });

        if (vetCareCost > 0)
            data.Add(new ExpensesPieChartDataPoint
                { Id = "vet", Label = "Obsługa weterynaryjna", Value = Math.Round(vetCareCost, 2) });

        if (otherCosts > 0)
            data.Add(new ExpensesPieChartDataPoint
                { Id = "other", Label = "Pozostałe", Value = Math.Round(otherCosts, 2) });

        return new DashboardExpensesPieChart { Data = data };
    }

    private static DashboardChickenHousesStatus BuildChickenHousesStatus(
        IReadOnlyList<FarmEntity> farms,
        IReadOnlyList<InsertionEntity> allInsertions,
        IReadOnlyList<SaleEntity> allSales,
        IReadOnlyList<ProductionDataFailureEntity> allFailures)
    {
        var farmStatuses = new List<DashboardFarmStatus>();

        // Tworzymy Lookups dla wydajności
        var insertionsLookup = allInsertions.ToLookup(i => i.HenhouseId);
        var salesLookup = allSales.ToLookup(s => s.HenhouseId);
        var failuresLookup = allFailures.ToLookup(f => f.HenhouseId);

        foreach (var farm in farms)
        {
            var farmStatus = new DashboardFarmStatus { Name = farm.Name };
            var activeCycleId = farm.ActiveCycleId;

            foreach (var henhouse in farm.Henhouses.Where(h => !h.DateDeletedUtc.HasValue))
            {
                var chickenCount = 0;
                if (activeCycleId.HasValue)
                {
                    var henhouseInsertions = insertionsLookup[henhouse.Id].Where(i => i.CycleId == activeCycleId);
                    var henhouseSales = salesLookup[henhouse.Id].Where(s => s.CycleId == activeCycleId);
                    var henhouseFailures = failuresLookup[henhouse.Id].Where(f => f.CycleId == activeCycleId);

                    var insertedCount = henhouseInsertions.Sum(i => i.Quantity);
                    var lossesCount = henhouseFailures.Sum(f => f.DeadCount + f.DefectiveCount);
                    var soldAndLostOnSaleCount = henhouseSales.Sum(s => s.Quantity + s.DeadCount + s.ConfiscatedCount);

                    chickenCount = insertedCount - (soldAndLostOnSaleCount + lossesCount);

                    if (insertedCount > 0 && chickenCount <= insertedCount * 0.01m)
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
    
    private record NotificationSource(DateOnly DueDate, NotificationType Type, object Entity);

    private async Task<List<DashboardNotificationItem>> BuildDashboardNotifications(CancellationToken ct)
    {

        var now = DateOnly.FromDateTime(DateTime.Now);
        var sevenDaysFromNow = now.AddDays(7);
        const int daysForMediumPriority = 3;

        var allSources = new List<NotificationSource>();

        // 1. Faktury Sprzedażowe (należności)
        var salesInvoices =
            await _saleInvoiceRepository.ListAsync(new GetOverdueAndUpcomingSaleInvoicesSpec(sevenDaysFromNow), ct);
        allSources.AddRange(salesInvoices.Select(inv =>
            new NotificationSource(inv.DueDate, NotificationType.SaleInvoice, inv)));

        // 2. Faktury za paszę (zobowiązania)
        var feedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetOverdueAndUpcomingFeedInvoicesSpec(sevenDaysFromNow), ct);
        allSources.AddRange(feedInvoices.Select(inv =>
            new NotificationSource(inv.DueDate, NotificationType.FeedInvoice, inv)));

        // 3. Kończące się umowy pracowników
        var expiringContracts =
            await _employeeRepository.ListAsync(new GetOverdueAndUpcomingEmployeesContractSpec(sevenDaysFromNow), ct);
        allSources.AddRange(expiringContracts.Select(emp =>
            new NotificationSource(emp.EndDate!.Value, NotificationType.EmployeeContract, emp)));

        // 4. Przypomnienia dla pracowników
        var employeeReminders =
            await _employeeReminderRepository.ListAsync(
                new GetOverdueAndUpcomingEmployeesRemindersSpec(now, sevenDaysFromNow), ct);
        allSources.AddRange(employeeReminders.Select(rem =>
            new NotificationSource(rem.DueDate, NotificationType.EmployeeReminder, rem)));

        // 5. Sortowanie, wybranie 5 najważniejszych i mapowanie do finalnego modelu
        var top5Notifications = allSources
            .OrderBy(s => s.DueDate) // Sortuj wg daty - zaległe i najbliższe będą pierwsze
            .Take(5)
            .Select(source =>
            {
                // Logika priorytetu
                NotificationPriority priority;
                if (source.DueDate <= now) priority = NotificationPriority.High;
                else if (source.DueDate <= now.AddDays(daysForMediumPriority)) priority = NotificationPriority.Medium;
                else priority = NotificationPriority.Low;

                // Logika generowania opisu
                var description = GenerateDescription(source, now);

                return new DashboardNotificationItem
                {
                    Description = description,
                    DueDate = source.DueDate,
                    Priority = priority,
                    Type = source.Type
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
            NotificationType.SaleInvoice =>
                $"Faktura sprzedaży: termin płatności {dateString}{dayDifference}",
            NotificationType.FeedInvoice =>
                $"Faktura za paszę: termin płatności {dateString}{dayDifference}",
            NotificationType.EmployeeContract =>
                $"Koniec umowy dla {(source.Entity as EmployeeEntity)?.FullName}: {dateString}{dayDifference}",
            NotificationType.EmployeeReminder =>
                $"Przypomnienie '{(source.Entity as EmployeeReminderEntity)?.Title}': {dateString}{dayDifference}",
            _ => "Nieznane powiadomienie"
        };
    }
}