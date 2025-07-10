using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Feeds;

public enum FeedsPricesOrderBy
{
    DateCreatedUtc,
    Cycle,
    Farm,
    PriceDate,
    Price,
    Name,
}

public record GetFeedsPricesQueryFilters : OrderedPaginationParams<FeedsPricesOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<CycleDictModel> Cycles { get; init; }
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetFeedsPricesQuery(GetFeedsPricesQueryFilters Filters)
    : IRequest<BaseResponse<GetFeedsPricesQueryResponse>>;

public record FeedPriceRowDto
{
    public Guid Id { get; init; }
    public string FarmName { get; init; }
    public string CycleText { get; init; }
    public DateOnly PriceDate { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetFeedsPricesQueryResponse : PaginationModel<FeedPriceRowDto>;

public class
    GetFeedsPricesQueryHandler : IRequestHandler<GetFeedsPricesQuery, BaseResponse<GetFeedsPricesQueryResponse>>
{
    private readonly IFeedPriceRepository _feedPriceRepository;

    public GetFeedsPricesQueryHandler(IFeedPriceRepository feedPriceRepository)
    {
        _feedPriceRepository = feedPriceRepository;
    }

    public async Task<BaseResponse<GetFeedsPricesQueryResponse>> Handle(GetFeedsPricesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _feedPriceRepository.ListAsync<FeedPriceRowDto>(
            new GetAllFeedsPricesSpec(request.Filters, true), cancellationToken);
        var count = await _feedPriceRepository.CountAsync(new GetAllFeedsPricesSpec(request.Filters, false),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetFeedsPricesQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class FeedsPricesProfile : Profile
{
    public FeedsPricesProfile()
    {
        CreateMap<FeedPriceEntity, FeedPriceRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name));
    }
}