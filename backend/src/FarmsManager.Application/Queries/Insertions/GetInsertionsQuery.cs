using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Insertions.Dictionary;
using MediatR;

namespace FarmsManager.Application.Queries.Insertions;

public enum InsertionOrderBy
{
    DateCreatedUtc,
    Cycle,
    Farm,
    Henhouse,
    InsertionDate,
    Quantity,
    Hatchery,
    BodyWeight,
}

public record GetInsertionsQueryFilters : OrderedPaginationParams<InsertionOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<Guid> HenhouseIds { get; init; }
    public List<Guid> HatcheryIds { get; init; }
    public List<CycleDictModel> Cycles { get; init; }
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetInsertionsQuery(GetInsertionsQueryFilters Filters)
    : IRequest<BaseResponse<GetInsertionsQueryResponse>>;