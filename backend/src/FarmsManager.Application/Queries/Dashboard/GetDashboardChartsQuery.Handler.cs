using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Users;
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

public class GetDashboardChartsQueryHandler : IRequestHandler<GetDashboardChartsQuery, BaseResponse<GetDashboardChartsQueryResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IInsertionRepository _insertionRepository;
    private readonly IProductionDataFailureRepository _productionDataFailureRepository;
    private readonly IGasConsumptionRepository _gasConsumptionRepository;

    public GetDashboardChartsQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver, IFarmRepository farmRepository, ISaleRepository saleRepository, IFeedInvoiceRepository feedInvoiceRepository, IInsertionRepository insertionRepository, IProductionDataFailureRepository productionDataFailureRepository, IGasConsumptionRepository gasConsumptionRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _saleRepository = saleRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _insertionRepository = insertionRepository;
        _productionDataFailureRepository = productionDataFailureRepository;
        _gasConsumptionRepository = gasConsumptionRepository;
    }

    public async Task<BaseResponse<GetDashboardChartsQueryResponse>> Handle(GetDashboardChartsQuery request, CancellationToken ct)
    {
        // 1. Walidacja użytkownika i pobranie farm (wspólne dla wszystkich wykresów)
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new GetDashboardChartsQueryResponse());
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
            return BaseResponse.CreateResponse(new GetDashboardChartsQueryResponse());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        // 2. Pobranie wszystkich potrzebnych danych historycznych w jednym miejscu
        var allSales = await _saleRepository.ListAsync(new GetSalesForDashboardSpec(farmIds), ct);
        var allFeeds = await _feedInvoiceRepository.ListAsync(new GetFeedsInvoicesForDashboardSpec(farmIds), ct);
        var allInsertions = await _insertionRepository.ListAsync(new InsertionsForFarmsSpec(farmIds), ct);
        var allFailures = await _productionDataFailureRepository.ListAsync(new ProductionDataFailuresForFarmsSpec(farmIds), ct);
        var allGasConsumptions = await _gasConsumptionRepository.ListAsync(new GasConsumptionsForFarmsSpec(farmIds), ct);

        // 3. Zbudowanie wszystkich wykresów
        var fcrChart = BuildFcrChart(farms, allSales, allFeeds);
        var ewwChart = BuildEwwChart(farms, allInsertions, allSales, allFeeds, allFailures);
        var gasConsumptionChart = BuildGasConsumptionChart(farms, allGasConsumptions);
        var flockLossChart = BuildFlockLossChart(farms, allFailures, allInsertions);

        // 4. Złożenie odpowiedzi
        return BaseResponse.CreateResponse(new GetDashboardChartsQueryResponse
        {
            FcrChart = fcrChart,
            EwwChart = ewwChart,
            GasConsumptionChart = gasConsumptionChart,
            FlockLossChart = flockLossChart
        });
    }

    // Poniżej znajdują się metody `Build...` skopiowane z poprzednich handlerów.
    // Są one `private static`, więc nie zanieczyszczają innych części aplikacji.

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
}