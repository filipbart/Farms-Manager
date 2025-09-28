
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public class GetExpensesPieChartQueryHandler : IRequestHandler<GetExpensesPieChartQuery, BaseResponse<DashboardExpensesPieChart>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IGasConsumptionRepository _gasConsumptionRepository;

    public GetExpensesPieChartQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver, IFarmRepository farmRepository, IFeedInvoiceRepository feedInvoiceRepository, IExpenseProductionRepository expenseProductionRepository, IGasConsumptionRepository gasConsumptionRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _gasConsumptionRepository = gasConsumptionRepository;
    }

    public async Task<BaseResponse<DashboardExpensesPieChart>> Handle(GetExpensesPieChartQuery request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new DashboardExpensesPieChart());
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
            return BaseResponse.CreateResponse(new DashboardExpensesPieChart());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        var allFeedInvoices = await _feedInvoiceRepository.ListAsync(new GetFeedsInvoicesForDashboardSpec(farmIds, null, request.Filters.DateSince, request.Filters.DateTo), ct);
        var allExpenses = await _expenseProductionRepository.ListAsync(new GetProductionExpensesForDashboardSpec(farmIds, null, request.Filters.DateSince, request.Filters.DateTo), ct);
        var allGasConsumptions = await _gasConsumptionRepository.ListAsync(new GasConsumptionsForFarmsSpec(farmIds), ct);
        var totalGasCost = allGasConsumptions.Sum(g => g.Cost);

        var expensesPieChart = BuildExpensesPieChart(allFeedInvoices, allExpenses, totalGasCost);

        return BaseResponse.CreateResponse(expensesPieChart);
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
}
