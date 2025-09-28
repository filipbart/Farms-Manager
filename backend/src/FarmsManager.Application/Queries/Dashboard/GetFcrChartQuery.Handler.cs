
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public class GetFcrChartQueryHandler : IRequestHandler<GetFcrChartQuery, BaseResponse<DashboardFcrChart>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;

    public GetFcrChartQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver,
        IFarmRepository farmRepository, ISaleRepository saleRepository, IFeedInvoiceRepository feedInvoiceRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _saleRepository = saleRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
    }

    public async Task<BaseResponse<DashboardFcrChart>> Handle(GetFcrChartQuery request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new DashboardFcrChart());
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
            return BaseResponse.CreateResponse(new DashboardFcrChart());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        var historicalSales = await _saleRepository.ListAsync(new GetSalesForDashboardSpec(farmIds), ct);
        var historicalFeedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetFeedsInvoicesForDashboardSpec(farmIds), ct);

        var fcrChart = BuildFcrChart(farms, historicalSales, historicalFeedInvoices);

        return BaseResponse.CreateResponse(fcrChart);
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
}
