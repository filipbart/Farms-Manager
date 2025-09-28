
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public record GetFlockLossChartQuery(GetDashboardDataQueryFilters Filters)
    : IRequest<BaseResponse<DashboardFlockLossChart>>;
