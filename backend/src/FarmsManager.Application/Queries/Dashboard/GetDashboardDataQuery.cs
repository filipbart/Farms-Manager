using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public record GetDashboardDataQueryFilters
{
    public Guid? FarmId { get; init; }
    public string Cycle { get; init; }

    public CycleDictModel CycleDict => Cycle.IsEmpty()
        ? null
        : new CycleDictModel
        {
            Identifier = int.Parse(Cycle.Split('-')[0]),
            Year = int.Parse(Cycle.Split('-')[1])
        };

    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
    public string DateCategory { get; init; }
}

public record GetDashboardDataQuery(GetDashboardDataQueryFilters Filters)
    : IRequest<BaseResponse<GetDashboardDataQueryResponse>>;