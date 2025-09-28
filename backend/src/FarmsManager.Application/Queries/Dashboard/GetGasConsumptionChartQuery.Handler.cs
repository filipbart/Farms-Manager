
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public class GetGasConsumptionChartQueryHandler : IRequestHandler<GetGasConsumptionChartQuery, BaseResponse<DashboardGasConsumptionChart>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly IGasConsumptionRepository _gasConsumptionRepository;

    public GetGasConsumptionChartQueryHandler(IUserRepository userRepository, IUserDataResolver userDataResolver, IFarmRepository farmRepository, IGasConsumptionRepository gasConsumptionRepository)
    {
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _gasConsumptionRepository = gasConsumptionRepository;
    }

    public async Task<BaseResponse<DashboardGasConsumptionChart>> Handle(GetGasConsumptionChartQuery request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        if (!user.IsAdmin && (user.Farms is null || user.Farms.Count == 0))
        {
            return BaseResponse.CreateResponse(new DashboardGasConsumptionChart());
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
            return BaseResponse.CreateResponse(new DashboardGasConsumptionChart());
        }

        var farmIds = farms.Select(f => f.Id).ToList();

        var allGasConsumptions = await _gasConsumptionRepository.ListAsync(new GasConsumptionsForFarmsSpec(farmIds), ct);

        var gasConsumptionChart = BuildGasConsumptionChart(farms, allGasConsumptions);

        return BaseResponse.CreateResponse(gasConsumptionChart);
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
}
