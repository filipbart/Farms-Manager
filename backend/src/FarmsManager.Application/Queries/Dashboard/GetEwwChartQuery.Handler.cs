
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public class GetEwwChartQueryHandler : IRequestHandler<GetEwwChartQuery, BaseResponse<DashboardEwwChart>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly IInsertionRepository _insertionRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IProductionDataFailureRepository _productionDataFailureRepository;

    public GetEwwChartQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver,
        IFarmRepository farmRepository, IInsertionRepository insertionRepository, ISaleRepository saleRepository,
        IFeedInvoiceRepository feedInvoiceRepository, IProductionDataFailureRepository productionDataFailureRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _insertionRepository = insertionRepository;
        _saleRepository = saleRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _productionDataFailureRepository = productionDataFailureRepository;
    }

    public async Task<BaseResponse<DashboardEwwChart>> Handle(GetEwwChartQuery request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new DashboardEwwChart());
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
            return BaseResponse.CreateResponse(new DashboardEwwChart());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        var allInsertions = await _insertionRepository.ListAsync(new InsertionsForFarmsSpec(farmIds), ct);
        var allSales = await _saleRepository.ListAsync(new GetSalesForDashboardSpec(farmIds), ct);
        var allFeeds = await _feedInvoiceRepository.ListAsync(new GetFeedsInvoicesForDashboardSpec(farmIds), ct);
        var allFailures = await _productionDataFailureRepository.ListAsync(new ProductionDataFailuresForFarmsSpec(farmIds), ct);

        var ewwChart = BuildEwwChart(farms, allInsertions, allSales, allFeeds, allFailures);

        return BaseResponse.CreateResponse(ewwChart);
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
}
