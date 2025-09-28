
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

// Reużywamy istniejący rekord z filtrami, chociaż ten wykres zdaje się używać tylko FarmId
public record GetFcrChartQuery(GetDashboardDataQueryFilters Filters)
    : IRequest<BaseResponse<DashboardFcrChart>>;
