
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

// Reużywamy istniejący rekord z filtrami
public record GetDashboardStatsQuery(GetDashboardDataQueryFilters Filters)
    : IRequest<BaseResponse<GetDashboardStatsQueryResponse>>;

public record GetDashboardStatsQueryResponse
{
    public DashboardStats Stats { get; set; }
    public DashboardChickenHousesStatus ChickenHousesStatus { get; set; }
}
