using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Hatcheries.Prices;

public enum HatcheriesPricesOrderBy
{
    HatcheryName,
    Price,
    Date
}

public record GetHatcheryPricesQueryFilters : OrderedPaginationParams<HatcheriesPricesOrderBy>
{
    public List<Guid> HatcheryIds { get; init; }
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
}

public record GetHatcheryPricesQuery(GetHatcheryPricesQueryFilters Filters)
    : IRequest<BaseResponse<GetHatcheryPricesQueryResponse>>;