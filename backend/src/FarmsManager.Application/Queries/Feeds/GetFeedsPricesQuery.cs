using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
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
    public List<string> FeedNames { get; init; }
    public List<string> Cycles { get; init; }

    public List<CycleDictModel> CyclesDict => Cycles?.Select(c => new CycleDictModel
    {
        Identifier = int.Parse(c.Split('-')[0]),
        Year = int.Parse(c.Split('-')[1])
    }).ToList();

    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetFeedsPricesQuery(GetFeedsPricesQueryFilters Filters)
    : IRequest<BaseResponse<GetFeedsPricesQueryResponse>>;

public record FeedPriceRowDto
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string FarmName { get; init; }
    public Guid CycleId { get; init; }
    public string CycleText { get; init; }
    public DateOnly PriceDate { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public class GetFeedsPricesQueryResponse : PaginationModel<FeedPriceRowDto>;

public class
    GetFeedsPricesQueryHandler : IRequestHandler<GetFeedsPricesQuery, BaseResponse<GetFeedsPricesQueryResponse>>
{
    private readonly IFeedPriceRepository _feedPriceRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetFeedsPricesQueryHandler(IFeedPriceRepository feedPriceRepository, IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _feedPriceRepository = feedPriceRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetFeedsPricesQueryResponse>> Handle(GetFeedsPricesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;
        var isAdmin = user.IsAdmin;

        var data = await _feedPriceRepository.ListAsync<FeedPriceRowDto>(
            new GetAllFeedsPricesSpec(request.Filters, true, accessibleFarmIds, isAdmin), cancellationToken);
        var count = await _feedPriceRepository.CountAsync(
            new GetAllFeedsPricesSpec(request.Filters, false, accessibleFarmIds, isAdmin),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetFeedsPricesQueryResponse
        {
            TotalRows = count,
            Items = data.ClearAdminData(isAdmin)
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