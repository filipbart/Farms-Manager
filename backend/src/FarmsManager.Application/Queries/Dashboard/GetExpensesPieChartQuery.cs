
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public record GetExpensesPieChartQuery(GetDashboardDataQueryFilters Filters)
    : IRequest<BaseResponse<DashboardExpensesPieChart>>;
