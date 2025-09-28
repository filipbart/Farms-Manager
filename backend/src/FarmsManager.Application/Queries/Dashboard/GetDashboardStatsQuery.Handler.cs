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
    GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery,
    BaseResponse<GetDashboardStatsQueryResponse>>
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

    public GetDashboardStatsQueryHandler(
        IUserRepository userRepository,
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        ISaleRepository saleRepository,
        ICycleRepository cycleRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        IGasConsumptionRepository gasConsumptionRepository,
        IExpenseProductionRepository expenseProductionRepository,
        IInsertionRepository insertionRepository,
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

    public async Task<BaseResponse<GetDashboardStatsQueryResponse>> Handle(
        GetDashboardStatsQuery request,
        CancellationToken ct)
    {
        // 1. Walidacja użytkownika i pobranie farm
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);

        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new GetDashboardStatsQueryResponse());
        }

        var accessibleFarmIds = user.AccessibleFarmIds;
        var filteredFarmIds = GetFilteredFarmIds(request, accessibleFarmIds);

        var farms = await _farmRepository.ListAsync(new GetAllFarmsSpec(filteredFarmIds), ct);
        if (farms.Count == 0)
        {
            return BaseResponse.CreateResponse(new GetDashboardStatsQueryResponse());
        }

        var farmIds = farms.Select(f => f.Id).ToList();
        var activeCycleIds = farms
            .Where(f => f.ActiveCycleId.HasValue)
            .Select(f => f.ActiveCycleId!.Value)
            .Distinct()
            .ToList();

        // 2. Pobierz cycle IDs dla filtrów (jeśli potrzebne)
        var cycleIdsForFilter = await GetFilteredCycleIds(request, farms, ct);

        // 3. Określ zakres dat dla różnych scenariuszy
        var (mainDateRange, kpiDateRange, kpiCycleIds) = DetermineDataRanges(
            request.Filters,
            cycleIdsForFilter,
            activeCycleIds);

        // 4. Wykonaj wszystkie zapytania równolegle
        var dataFetchTask = FetchAllDataAsync(
            farmIds,
            cycleIdsForFilter,
            activeCycleIds,
            mainDateRange,
            kpiDateRange,
            kpiCycleIds,
            request.Filters.DateCategory,
            ct);

        var allData = await dataFetchTask;

        // 5. Przetwórz dane dla głównych statystyk
        var (gasCostForStats, otherExpenses) = ProcessExpensesForMainStats(
            allData.FilteredExpenses,
            allData.GasConsumptions,
            cycleIdsForFilter,
            request.Filters.DateCategory);

        // 6. Oblicz KPIs
        var (incomePerKg, incomePerSqm) = CalculateIncomeKpis(
            allData.KpiSales,
            allData.KpiFeedInvoices,
            allData.KpiOtherExpenses,
            allData.KpiGasCost,
            farms);

        // 7. Zbuduj odpowiedź
        var stats = BuildDashboardStats(
            allData.FilteredSales,
            allData.FilteredFeedInvoices,
            otherExpenses,
            gasCostForStats,
            incomePerKg,
            incomePerSqm);

        var chickenHousesStatus = BuildChickenHousesStatus(
            farms,
            allData.Insertions,
            allData.StatusSales,
            allData.Failures);

        return BaseResponse.CreateResponse(new GetDashboardStatsQueryResponse
        {
            Stats = stats,
            ChickenHousesStatus = chickenHousesStatus
        });
    }

    private static List<Guid> GetFilteredFarmIds(GetDashboardStatsQuery request, List<Guid> accessibleFarmIds)
    {
        if (!request.Filters.FarmId.HasValue)
            return accessibleFarmIds;

        if (accessibleFarmIds != null && !accessibleFarmIds.Contains(request.Filters.FarmId.Value))
            throw DomainException.Forbidden();

        return [request.Filters.FarmId.Value];
    }

    private static (DateRange mainRange, DateRange kpiRange, List<Guid> kpiCycleIds) DetermineDataRanges(
        GetDashboardDataQueryFilters filters,
        List<Guid> cycleIdsForFilter,
        List<Guid> activeCycleIds)
    {
        var mainRange = new DateRange(filters.DateSince, filters.DateTo);

        switch (filters.DateCategory?.ToLower())
        {
            case "year":
                var year = filters.DateSince!.Value.Year;
                var yearRange = new DateRange(
                    new DateOnly(year, 1, 1),
                    new DateOnly(year, 12, 31));
                return (mainRange, yearRange, null);

            case "month":
                return (mainRange, DateRange.Empty, activeCycleIds);

            default:
                return (mainRange, mainRange, cycleIdsForFilter.Count != 0 ? cycleIdsForFilter : null);
        }
    }

    private async Task<DashboardData> FetchAllDataAsync(
        List<Guid> farmIds,
        List<Guid> cycleIdsForFilter,
        List<Guid> activeCycleIds,
        DateRange mainRange,
        DateRange kpiRange,
        List<Guid> kpiCycleIds,
        string dateCategory,
        CancellationToken ct)
    {
        // Przygotuj listę tasków
        var tasks = new List<Task>();

        // Dane główne (dla statystyk)
        var mainSalesTask = _saleRepository.ListAsync(
            new GetSalesForDashboardSpec(farmIds, cycleIdsForFilter, mainRange.Since, mainRange.To), ct);
        tasks.Add(mainSalesTask);

        var mainFeedsTask = _feedInvoiceRepository.ListAsync(
            new GetFeedsInvoicesForDashboardSpec(farmIds, cycleIdsForFilter, mainRange.Since, mainRange.To), ct);
        tasks.Add(mainFeedsTask);

        var mainExpensesTask = _expenseProductionRepository.ListAsync(
            new GetProductionExpensesForDashboardSpec(farmIds, cycleIdsForFilter, mainRange.Since, mainRange.To), ct);
        tasks.Add(mainExpensesTask);

        // Gas consumptions (tylko dla cycle)
        Task<List<GasConsumptionEntity>> gasConsumptionsTask = null;
        if (string.Equals(dateCategory, "cycle", StringComparison.OrdinalIgnoreCase) && cycleIdsForFilter.Count != 0)
        {
            gasConsumptionsTask = _gasConsumptionRepository.ListAsync(
                new GasConsumptionsForDashboardSpec(farmIds, cycleIdsForFilter), ct);
            tasks.Add(gasConsumptionsTask);
        }

        // Dane dla KPI (mogą być te same co główne lub inne)
        Task<List<SaleEntity>> kpiSalesTask = null;
        Task<List<FeedInvoiceEntity>> kpiFeedsTask = null;
        Task<List<ExpenseProductionEntity>> kpiExpensesTask = null;

        if (!kpiRange.IsEmpty && kpiRange != mainRange)
        {
            kpiSalesTask = _saleRepository.ListAsync(
                new GetSalesForDashboardSpec(farmIds, null, kpiRange.Since, kpiRange.To), ct);
            tasks.Add(kpiSalesTask);

            kpiFeedsTask = _feedInvoiceRepository.ListAsync(
                new GetFeedsInvoicesForDashboardSpec(farmIds, null, kpiRange.Since, kpiRange.To), ct);
            tasks.Add(kpiFeedsTask);

            kpiExpensesTask = _expenseProductionRepository.ListAsync(
                new GetProductionExpensesForDashboardSpec(farmIds, null, kpiRange.Since, kpiRange.To), ct);
            tasks.Add(kpiExpensesTask);
        }
        else if (kpiCycleIds?.Any() == true)
        {
            kpiSalesTask = _saleRepository.ListAsync(
                new GetSalesForDashboardSpec(farmIds, kpiCycleIds), ct);
            tasks.Add(kpiSalesTask);

            kpiFeedsTask = _feedInvoiceRepository.ListAsync(
                new GetFeedsInvoicesForDashboardSpec(farmIds, kpiCycleIds), ct);
            tasks.Add(kpiFeedsTask);

            kpiExpensesTask = _expenseProductionRepository.ListAsync(
                new GetProductionExpensesForDashboardSpec(farmIds, kpiCycleIds), ct);
            tasks.Add(kpiExpensesTask);
        }

        // Dane dla statusu kurników (tylko aktywne cykle)
        Task<List<InsertionEntity>> insertionsTask = null;
        Task<List<SaleEntity>> statusSalesTask = null;
        Task<List<ProductionDataFailureEntity>> failuresTask = null;

        if (activeCycleIds.Count != 0)
        {
            insertionsTask = _insertionRepository.ListAsync(
                new InsertionsForFarmsSpec(farmIds, activeCycleIds), ct);
            tasks.Add(insertionsTask);

            statusSalesTask = _saleRepository.ListAsync(
                new GetSalesForDashboardSpec(farmIds, activeCycleIds), ct);
            tasks.Add(statusSalesTask);

            failuresTask = _productionDataFailureRepository.ListAsync(
                new ProductionDataFailuresForFarmsSpec(farmIds, activeCycleIds), ct);
            tasks.Add(failuresTask);
        }

        // Wykonaj wszystkie zapytania równolegle
        await Task.WhenAll(tasks);

        // Zbierz wyniki
        var mainSales = await mainSalesTask;
        var mainFeeds = await mainFeedsTask;
        var mainExpenses = await mainExpensesTask;
        var gasConsumptions =
            gasConsumptionsTask != null ? await gasConsumptionsTask : [];

        // KPI data - użyj głównych danych jeśli nie ma osobnych
        var kpiSales = kpiSalesTask != null ? await kpiSalesTask : mainSales;
        var kpiFeeds = kpiFeedsTask != null ? await kpiFeedsTask : mainFeeds;
        var kpiExpenses = kpiExpensesTask != null ? await kpiExpensesTask : mainExpenses;

        // Przetwórz koszty gazu dla KPI
        var (kpiGasExpenses, kpiOtherExpenses) = SplitGasExpenses(kpiExpenses);
        var kpiGasCost = kpiGasExpenses.Sum(e => e.SubTotal);

        return new DashboardData
        {
            FilteredSales = mainSales,
            FilteredFeedInvoices = mainFeeds,
            FilteredExpenses = mainExpenses,
            GasConsumptions = gasConsumptions,
            KpiSales = kpiSales,
            KpiFeedInvoices = kpiFeeds,
            KpiOtherExpenses = kpiOtherExpenses,
            KpiGasCost = kpiGasCost,
            Insertions = insertionsTask != null ? await insertionsTask : [],
            StatusSales = statusSalesTask != null ? await statusSalesTask : [],
            Failures = failuresTask != null ? await failuresTask : []
        };
    }

    private (decimal gasCost, IReadOnlyList<ExpenseProductionEntity> otherExpenses) ProcessExpensesForMainStats(
        IReadOnlyList<ExpenseProductionEntity> allExpenses,
        IReadOnlyList<GasConsumptionEntity> gasConsumptions,
        List<Guid> cycleIdsForFilter,
        string dateCategory)
    {
        if (string.Equals(dateCategory, "cycle", StringComparison.OrdinalIgnoreCase) && cycleIdsForFilter.Count != 0)
        {
            var gasCost = gasConsumptions.Sum(t => t.Cost);
            return (gasCost, allExpenses);
        }

        var (gasExpenses, otherExpenses) = SplitGasExpenses(allExpenses);
        return (gasExpenses.Sum(e => e.SubTotal), otherExpenses);
    }

    private async Task<List<Guid>> GetFilteredCycleIds(
        GetDashboardStatsQuery request,
        List<FarmEntity> farms,
        CancellationToken ct)
    {
        if (request.Filters.CycleDict == null)
            return [];

        // Wykonaj zapytania równolegle dla wszystkich farm
        var tasks = farms.Select(farm =>
            _cycleRepository.FirstOrDefaultAsync(
                new GetCycleByYearIdentifierAndFarmSpec(
                    farm.Id,
                    request.Filters.CycleDict.Year,
                    request.Filters.CycleDict.Identifier),
                ct)
        ).ToList();

        var cycles = await Task.WhenAll(tasks);
        return cycles.Where(c => c != null).Select(c => c!.Id).ToList();
    }

    private static (IReadOnlyList<ExpenseProductionEntity> gasExpenses, IReadOnlyList<ExpenseProductionEntity>
        otherExpenses)
        SplitGasExpenses(IReadOnlyList<ExpenseProductionEntity> allExpenses)
    {
        var gasExpenses = new List<ExpenseProductionEntity>();
        var otherExpenses = new List<ExpenseProductionEntity>();

        foreach (var expense in allExpenses)
        {
            if (string.Equals(expense.ExpenseContractor?.ExpenseType?.Name, "Gaz", StringComparison.OrdinalIgnoreCase))
                gasExpenses.Add(expense);
            else
                otherExpenses.Add(expense);
        }

        return (gasExpenses, otherExpenses);
    }

    private static (decimal incomePerKg, decimal incomePerSqm) CalculateIncomeKpis(
        IReadOnlyList<SaleEntity> sales,
        IReadOnlyList<FeedInvoiceEntity> feedInvoices,
        IReadOnlyList<ExpenseProductionEntity> expenses,
        decimal gasCost,
        IReadOnlyList<FarmEntity> farms)
    {
        var feedCosts = feedInvoices.Sum(t => t.SubTotal);
        var expensesCost = expenses.Sum(t => t.SubTotal);
        var totalRevenue = sales.Sum(s => (s.Weight - s.DeadWeight - s.ConfiscatedWeight) * s.PriceWithExtras);
        var netIncome = totalRevenue - feedCosts - gasCost - expensesCost;

        var totalWeight = sales.Sum(t => t.Weight);
        var totalArea = farms.Sum(f => f.Henhouses?.Sum(h => h.Area) ?? 0);

        var incomePerKg = totalWeight > 0 ? netIncome / totalWeight : 0;
        var incomePerSqm = totalArea > 0 ? netIncome / totalArea : 0;

        return (incomePerKg, incomePerSqm);
    }

    private static DashboardStats BuildDashboardStats(
        IReadOnlyList<SaleEntity> sales,
        IReadOnlyList<FeedInvoiceEntity> feedInvoices,
        IReadOnlyList<ExpenseProductionEntity> otherExpenses,
        decimal gasCost,
        decimal incomePerKg,
        decimal incomePerSqm)
    {
        var feedCosts = feedInvoices.Sum(t => t.SubTotal);
        var expensesCost = otherExpenses.Sum(t => t.SubTotal);
        var totalRevenue = sales.Sum(s => (s.Weight - s.DeadWeight - s.ConfiscatedWeight) * s.PriceWithExtras);
        var sumExpenses = feedCosts + gasCost + expensesCost;

        var feedVat = feedInvoices.Sum(t => t.VatAmount);
        var expensesVat = otherExpenses.Sum(e => e.VatAmount);
        var gasVat = gasCost * 0.23m;
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

        // Grupuj dane raz dla lepszej wydajności
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
                    var insertedCount = insertionsLookup[henhouse.Id]
                        .Where(i => i.CycleId == activeCycleId)
                        .Sum(i => i.Quantity);

                    var lossesCount = failuresLookup[henhouse.Id]
                        .Where(f => f.CycleId == activeCycleId)
                        .Sum(f => f.DeadCount + f.DefectiveCount);

                    var soldCount = salesLookup[henhouse.Id]
                        .Where(s => s.CycleId == activeCycleId)
                        .Sum(s => s.Quantity);

                    chickenCount = insertedCount - soldCount - lossesCount;

                    // Jeśli pozostało mniej niż 4% z początkowej ilości, uznajemy że kurnik jest pusty
                    if (insertedCount > 0 && chickenCount <= insertedCount * 0.04m)
                    {
                        chickenCount = 0;
                    }
                }

                farmStatus.Henhouses.Add(new DashboardHenhouseStatus
                {
                    Name = henhouse.Name,
                    ChickenCount = Math.Max(0, chickenCount)
                });
            }

            farmStatuses.Add(farmStatus);
        }

        return new DashboardChickenHousesStatus { Farms = farmStatuses };
    }

    // Klasy pomocnicze
    private class DateRange(DateOnly? since, DateOnly? to)
    {
        public DateOnly? Since { get; } = since;
        public DateOnly? To { get; } = to;
        public bool IsEmpty => !Since.HasValue && !To.HasValue;

        public static DateRange Empty => new DateRange(null, null);

        public static bool operator ==(DateRange a, DateRange b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Since == b.Since && a.To == b.To;
        }

        public static bool operator !=(DateRange a, DateRange b) => !(a == b);

        public override bool Equals(object obj) => obj is DateRange range && this == range;
        public override int GetHashCode() => HashCode.Combine(Since, To);
    }

    private class DashboardData
    {
        public IReadOnlyList<SaleEntity> FilteredSales { get; init; } = new List<SaleEntity>();
        public IReadOnlyList<FeedInvoiceEntity> FilteredFeedInvoices { get; init; } = new List<FeedInvoiceEntity>();

        public IReadOnlyList<ExpenseProductionEntity> FilteredExpenses { get; init; } =
            new List<ExpenseProductionEntity>();

        public IReadOnlyList<GasConsumptionEntity> GasConsumptions { get; init; } = new List<GasConsumptionEntity>();

        public IReadOnlyList<SaleEntity> KpiSales { get; init; } = new List<SaleEntity>();
        public IReadOnlyList<FeedInvoiceEntity> KpiFeedInvoices { get; init; } = new List<FeedInvoiceEntity>();

        public IReadOnlyList<ExpenseProductionEntity> KpiOtherExpenses { get; init; } =
            new List<ExpenseProductionEntity>();

        public decimal KpiGasCost { get; init; }

        public IReadOnlyList<InsertionEntity> Insertions { get; init; } = new List<InsertionEntity>();
        public IReadOnlyList<SaleEntity> StatusSales { get; init; } = new List<SaleEntity>();

        public IReadOnlyList<ProductionDataFailureEntity> Failures { get; init; } =
            new List<ProductionDataFailureEntity>();
    }
}