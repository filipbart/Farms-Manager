
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public record GetEwwChartQuery(GetDashboardDataQueryFilters Filters)
    : IRequest<BaseResponse<DashboardEwwChart>>;
