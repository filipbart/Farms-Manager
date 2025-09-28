
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public class GetFlockLossChartQueryHandler : IRequestHandler<GetFlockLossChartQuery, BaseResponse<DashboardFlockLossChart>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly IProductionDataFailureRepository _productionDataFailureRepository;
    private readonly IInsertionRepository _insertionRepository;

    public GetFlockLossChartQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver, IFarmRepository farmRepository, IProductionDataFailureRepository productionDataFailureRepository, IInsertionRepository insertionRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _productionDataFailureRepository = productionDataFailureRepository;
        _insertionRepository = insertionRepository;
    }

    public async Task<BaseResponse<DashboardFlockLossChart>> Handle(GetFlockLossChartQuery request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new DashboardFlockLossChart());
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
            return BaseResponse.CreateResponse(new DashboardFlockLossChart());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        var allFailures = await _productionDataFailureRepository.ListAsync(new ProductionDataFailuresForFarmsSpec(farmIds), ct);
        var allInsertions = await _insertionRepository.ListAsync(new InsertionsForFarmsSpec(farmIds), ct);

        var flockLossChart = BuildFlockLossChart(farms, allFailures, allInsertions);

        return BaseResponse.CreateResponse(flockLossChart);
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
