using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Weighings;

public enum ProductionDataWeighingsOrderBy
{
    DateCreatedUtc,
    Cycle,
    Farm,
    Henhouse,
    Hatchery,
    Weighing1Weight,
    Weighing2Weight,
    Weighing3Weight,
    Weighing4Weight,
    Weighing5Weight
}

public record GetProductionDataWeighingsQueryFilters : OrderedPaginationParams<ProductionDataWeighingsOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<Guid> HenhouseIds { get; init; }
    public List<Guid> HatcheryIds { get; init; }
    public List<string> Cycles { get; init; }

    public List<CycleDictModel> CyclesDict => Cycles?.Select(c => new CycleDictModel
    {
        Identifier = int.Parse(c.Split('-')[0]),
        Year = int.Parse(c.Split('-')[1])
    }).ToList();

    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetProductionDataWeighingsQuery(GetProductionDataWeighingsQueryFilters Filters)
    : IRequest<BaseResponse<GetProductionDataWeighingsQueryResponse>>;