
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public record GetGasConsumptionChartQuery(GetDashboardDataQueryFilters Filters)
    : IRequest<BaseResponse<DashboardGasConsumptionChart>>;
