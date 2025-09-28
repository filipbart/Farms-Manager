
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public record GetDashboardNotificationsQuery(GetDashboardDataQueryFilters Filters)
    : IRequest<BaseResponse<List<DashboardNotificationItem>>>;
