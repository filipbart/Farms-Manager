using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.FlockLosses;

public enum ProductionDataFlockLossesOrderBy
{
    DateCreatedUtc,
    Cycle,
    Farm,
    Henhouse,
    Hatchery,
    FlockLoss1Percentage,
    FlockLoss2Percentage,
    FlockLoss3Percentage,
    FlockLoss4Percentage
}

public record GetProductionDataFlockLossesQueryFilters : OrderedPaginationParams<ProductionDataFlockLossesOrderBy>
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

public record GetProductionDataFlockLossesQuery(GetProductionDataFlockLossesQueryFilters Filters)
    : IRequest<BaseResponse<GetProductionDataFlockLossesQueryResponse>>;