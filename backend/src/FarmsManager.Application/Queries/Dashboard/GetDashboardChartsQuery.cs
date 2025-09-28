using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public record GetDashboardChartsQuery(GetDashboardDataQueryFilters Filters)
    : IRequest<BaseResponse<GetDashboardChartsQueryResponse>>;

public record GetDashboardChartsQueryResponse
{
    public DashboardFcrChart FcrChart { get; set; }
    public DashboardEwwChart EwwChart { get; set; }
    public DashboardGasConsumptionChart GasConsumptionChart { get; set; }
    public DashboardFlockLossChart FlockLossChart { get; set; }
}