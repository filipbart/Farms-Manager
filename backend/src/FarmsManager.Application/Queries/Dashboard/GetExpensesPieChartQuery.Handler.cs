using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.FallenStock;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public class
    GetExpensesPieChartQueryHandler : IRequestHandler<GetExpensesPieChartQuery, BaseResponse<DashboardExpensesPieChart>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IGasConsumptionRepository _gasConsumptionRepository;
    private readonly ICycleRepository _cycleRepository;

    // Stałe dla typów kosztów
    private const string ExpenseTypeChicks = "Zakup piskląt";
    private const string ExpenseTypeVet = "Usługa weterynaryjna";
    private const string ExpenseTypeGas = "Gaz";

    public GetExpensesPieChartQueryHandler(
        IUserRepository userRepository,
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        IExpenseProductionRepository expenseProductionRepository,
        IGasConsumptionRepository gasConsumptionRepository,
        ICycleRepository cycleRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _gasConsumptionRepository = gasConsumptionRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<BaseResponse<DashboardExpensesPieChart>> Handle(GetExpensesPieChartQuery request,
        CancellationToken ct)
    {
        // 1. Walidacja użytkownika i pobranie farm
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);

        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new DashboardExpensesPieChart());
        }

        var accessibleFarmIds = user.AccessibleFarmIds;
        var filteredFarmIds = GetFilteredFarmIds(request.Filters.FarmId, accessibleFarmIds);

        var farms = await _farmRepository.ListAsync(new GetAllFarmsSpec(filteredFarmIds), ct);
        if (farms.Count == 0)
        {
            return BaseResponse.CreateResponse(new DashboardExpensesPieChart());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        // 2. Obsługa filtrów cyklu
        List<Guid> cycleIds = null;
        if (request.Filters.CycleDict != null)
        {
            cycleIds = await GetCycleIdsForFilter(request.Filters.CycleDict, farms, ct);
            if (cycleIds.Count == 0)
            {
                return BaseResponse.CreateResponse(new DashboardExpensesPieChart());
            }
        }

        // 3. Określ strategię pobierania kosztów gazu
        var useGasConsumptions = ShouldUseGasConsumptions(request.Filters.DateCategory, cycleIds);

        // 4. Wykonaj wszystkie zapytania równolegle
        var expenseData = await FetchExpenseDataAsync(
            farmIds,
            cycleIds,
            request.Filters.DateSince,
            request.Filters.DateTo,
            useGasConsumptions,
            ct);

        // 5. Zbuduj wykres
        var expensesPieChart = BuildExpensesPieChart(
            expenseData.FeedInvoices,
            expenseData.Expenses,
            expenseData.GasCost,
            useGasConsumptions);

        return BaseResponse.CreateResponse(expensesPieChart);
    }

    private static List<Guid> GetFilteredFarmIds(Guid? farmIdFilter, List<Guid> accessibleFarmIds)
    {
        if (!farmIdFilter.HasValue)
            return accessibleFarmIds;

        if (accessibleFarmIds != null && !accessibleFarmIds.Contains(farmIdFilter.Value))
            throw DomainException.Forbidden();

        var ids = new List<Guid> { farmIdFilter.Value };
        return ids;
    }

    private async Task<List<Guid>> GetCycleIdsForFilter(
        CycleDictModel cycleDict,
        List<FarmEntity> farms,
        CancellationToken ct)
    {
        // Wykonaj zapytania równolegle dla wszystkich farm
        var tasks = farms.Select(farm =>
            _cycleRepository.FirstOrDefaultAsync(
                new GetCycleByYearIdentifierAndFarmSpec(
                    farm.Id,
                    cycleDict.Year,
                    cycleDict.Identifier),
                ct)
        ).ToList();

        var cycles = await Task.WhenAll(tasks);
        return cycles.Where(c => c != null).Select(c => c!.Id).ToList();
    }

    private static bool ShouldUseGasConsumptions(string dateCategory, List<Guid> cycleIds)
    {
        // Używamy GasConsumptions tylko gdy filtrujemy po cyklach
        return string.Equals(dateCategory, "cycle", StringComparison.OrdinalIgnoreCase)
               && cycleIds?.Any() == true;
    }

    private async Task<ExpenseData> FetchExpenseDataAsync(
        List<Guid> farmIds,
        List<Guid> cycleIds,
        DateOnly? dateSince,
        DateOnly? dateTo,
        bool useGasConsumptions,
        CancellationToken ct)
    {
        // Przygotuj zadania
        var feedInvoicesTask = _feedInvoiceRepository.ListAsync(
            new GetFeedsInvoicesForDashboardSpec(farmIds, cycleIds, dateSince, dateTo), ct);

        var expensesTask = _expenseProductionRepository.ListAsync(
            new GetProductionExpensesForDashboardSpec(farmIds, cycleIds, dateSince, dateTo), ct);

        Task<List<GasConsumptionEntity>> gasConsumptionsTask = null;
        if (useGasConsumptions)
        {
            // Użyj specyfikacji z filtrami cyklu jeśli są dostępne
            gasConsumptionsTask = cycleIds?.Any() == true
                ? _gasConsumptionRepository.ListAsync(
                    new GasConsumptionsForDashboardSpec(farmIds, cycleIds), ct)
                : _gasConsumptionRepository.ListAsync(
                    new GasConsumptionsForFarmsSpec(farmIds), ct);
        }

        // Wykonaj wszystkie zapytania równolegle
        var tasks = new List<Task>
        {
            feedInvoicesTask,
            expensesTask
        };
        if (gasConsumptionsTask != null)
            tasks.Add(gasConsumptionsTask);

        await Task.WhenAll(tasks);

        // Zbierz wyniki
        var feedInvoices = await feedInvoicesTask;
        var expenses = await expensesTask;

        decimal gasCost;
        if (gasConsumptionsTask != null)
        {
            var gasConsumptions = await gasConsumptionsTask;
            gasCost = gasConsumptions.Sum(g => g.Cost);
        }
        else
        {
            // Jeśli nie używamy GasConsumptions, wyciągnij koszty gazu z expenses
            gasCost = expenses
                .Where(e => string.Equals(e.ExpenseContractor?.ExpenseType?.Name, ExpenseTypeGas,
                    StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.SubTotal);
        }

        return new ExpenseData
        {
            FeedInvoices = feedInvoices,
            Expenses = expenses,
            GasCost = gasCost
        };
    }

    private static DashboardExpensesPieChart BuildExpensesPieChart(
        IReadOnlyList<FeedInvoiceEntity> allFeedInvoices,
        IReadOnlyList<ExpenseProductionEntity> allExpenses,
        decimal gasCost,
        bool gasFromConsumptions)
    {
        // Oblicz koszty paszy
        var feedCost = allFeedInvoices.Sum(f => f.SubTotal);

        // Kategoryzuj wydatki w jednej iteracji
        var expensesByCategory = CategorizeExpenses(allExpenses, gasFromConsumptions);

        // Oblicz sumę całkowitą
        var totalExpenses = feedCost + gasCost + expensesByCategory.ChicksCost +
                            expensesByCategory.VetCareCost + expensesByCategory.OtherCosts;

        if (totalExpenses == 0)
        {
            return new DashboardExpensesPieChart { Data = new List<ExpensesPieChartDataPoint>() };
        }

        // Buduj punkty wykresu
        var data = new List<ExpensesPieChartDataPoint>();

        AddDataPointIfPositive(data, "feed", "Pasza", feedCost, totalExpenses);
        AddDataPointIfPositive(data, "gas", "Gaz", gasCost, totalExpenses);
        AddDataPointIfPositive(data, "chicks", "Pisklęta", expensesByCategory.ChicksCost, totalExpenses);
        AddDataPointIfPositive(data, "vet", "Obsługa wet.", expensesByCategory.VetCareCost, totalExpenses);
        AddDataPointIfPositive(data, "other", "Pozostałe", expensesByCategory.OtherCosts, totalExpenses);

        return new DashboardExpensesPieChart
        {
            Data = data.OrderByDescending(d => d.Value).ToList()
        };
    }

    private static ExpenseCategories CategorizeExpenses(
        IReadOnlyList<ExpenseProductionEntity> expenses,
        bool excludeGas)
    {
        var result = new ExpenseCategories();

        foreach (var expense in expenses)
        {
            var expenseType = expense.ExpenseContractor?.ExpenseType?.Name ?? string.Empty;
            var amount = expense.SubTotal;

            if (excludeGas && string.Equals(expenseType, ExpenseTypeGas, StringComparison.OrdinalIgnoreCase))
            {
                // Pomijamy gaz jeśli jest liczony z GasConsumptions
            }
            else if (string.Equals(expenseType, ExpenseTypeChicks, StringComparison.OrdinalIgnoreCase))
            {
                result.ChicksCost += amount;
            }
            else if (string.Equals(expenseType, ExpenseTypeVet, StringComparison.OrdinalIgnoreCase))
            {
                result.VetCareCost += amount;
            }
            else if (!excludeGas || !string.Equals(expenseType, ExpenseTypeGas, StringComparison.OrdinalIgnoreCase))
            {
                result.OtherCosts += amount;
            }
        }

        return result;
    }

    private static void AddDataPointIfPositive(
        List<ExpensesPieChartDataPoint> data,
        string id,
        string label,
        decimal value,
        decimal total)
    {
        if (value > 0)
        {
            data.Add(new ExpensesPieChartDataPoint
            {
                Id = id,
                Label = label,
                Value = Math.Round(value / total * 100, 2)
            });
        }
    }

    // Klasy pomocnicze
    private class ExpenseData
    {
        public IReadOnlyList<FeedInvoiceEntity> FeedInvoices { get; init; } = new List<FeedInvoiceEntity>();
        public IReadOnlyList<ExpenseProductionEntity> Expenses { get; init; } = new List<ExpenseProductionEntity>();
        public decimal GasCost { get; init; }
    }

    private class ExpenseCategories
    {
        public decimal ChicksCost { get; set; }
        public decimal VetCareCost { get; set; }
        public decimal OtherCosts { get; set; }
    }
}