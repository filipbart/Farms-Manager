using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Failures;

public enum ProductionDataFailureOrderBy
{
    DateCreatedUtc,
    Cycle,
    Farm,
    Henhouse,
    DeadCount,
    DefectiveCount
}

public record GetProductionDataFailuresQueryFilters : OrderedPaginationParams<ProductionDataFailureOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<Guid> HenhouseIds { get; init; }
    public List<CycleDictModel> Cycles { get; init; }
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetProductionDataFailuresQuery(GetProductionDataFailuresQueryFilters Filters)
    : IRequest<BaseResponse<GetProductionDataFailuresQueryResponse>>;