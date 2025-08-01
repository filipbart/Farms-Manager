using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using MediatR;

namespace FarmsManager.Application.Queries.Gas.Consumptions;

public enum GasConsumptionsOrderBy
{
    Cycle,
    Farm,
    QuantityConsumed,
    Cost
}

public record GetGasConsumptionsQueryFilters : OrderedPaginationParams<GasConsumptionsOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<CycleDictModel> Cycles { get; init; }
}

public record GetGasConsumptionsQuery(GetGasConsumptionsQueryFilters Filters)
    : IRequest<BaseResponse<GetGasConsumptionsQueryResponse>>;