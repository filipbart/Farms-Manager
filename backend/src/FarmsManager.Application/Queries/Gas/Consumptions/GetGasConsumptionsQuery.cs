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
    public List<string> Cycles { get; init; }

    public List<CycleDictModel> CyclesDict => Cycles?.Select(c => new CycleDictModel
    {
        Identifier = int.Parse(c.Split('-')[0]),
        Year = int.Parse(c.Split('-')[1])
    }).ToList();
}

public record GetGasConsumptionsQuery(GetGasConsumptionsQueryFilters Filters)
    : IRequest<BaseResponse<GetGasConsumptionsQueryResponse>>;