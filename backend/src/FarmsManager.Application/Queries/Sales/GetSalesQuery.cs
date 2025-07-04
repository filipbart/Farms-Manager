using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using MediatR;

namespace FarmsManager.Application.Queries.Sales;

public enum SaleOrderBy
{
    DateCreatedUtc,
    Cycle,
    Farm,
    Henhouse,
    SaleDate,
    Quantity,
    Weight,
}

public record GetSalesQueryFilters : OrderedPaginationParams<SaleOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<Guid> HenhouseIds { get; init; }
    public List<Guid> SlaughterhouseIds { get; init; }
    public List<CycleDictModel> Cycles { get; init; }
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetSalesQuery(GetSalesQueryFilters Filters)
    : IRequest<BaseResponse<GetSalesQueryResponse>>;