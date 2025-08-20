using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
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

public enum FeedsPaymentsOrderBy
{
    DateCreatedUtc,
}

public record GetFeedsPaymentsQueryFilters : OrderedPaginationParams<FeedsPaymentsOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<string> Cycles { get; init; }

    public List<CycleDictModel> CyclesDict => Cycles?.Select(c => new CycleDictModel
    {
        Identifier = int.Parse(c.Split('-')[0]),
        Year = int.Parse(c.Split('-')[1])
    }).ToList();
}

public record GetFeedsPaymentsQuery(GetFeedsPaymentsQueryFilters Filters)
    : IRequest<BaseResponse<GetFeedsPaymentsQueryResponse>>;

public record FeedPaymentRowDto
{
    public Guid Id { get; init; }
    public string FarmName { get; init; }
    public string CycleText { get; init; }
    public string FilePath { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetFeedsPaymentsQueryResponse : PaginationModel<FeedPaymentRowDto>;

public class
    GetFeedsPaymentsQueryHandler : IRequestHandler<GetFeedsPaymentsQuery,
    BaseResponse<GetFeedsPaymentsQueryResponse>>
{
    private readonly IFeedPaymentRepository _feedPaymentRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetFeedsPaymentsQueryHandler(IFeedPaymentRepository feedPaymentRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _feedPaymentRepository = feedPaymentRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetFeedsPaymentsQueryResponse>> Handle(GetFeedsPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.IsAdmin ? null : user.Farms?.Select(t => t.FarmId).ToList();

        var data = await _feedPaymentRepository.ListAsync<FeedPaymentRowDto>(
            new GetAllFeedsPaymentsSpec(request.Filters, true, accessibleFarmIds), cancellationToken);
        var count = await _feedPaymentRepository.CountAsync(
            new GetAllFeedsPaymentsSpec(request.Filters, false, accessibleFarmIds),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetFeedsPaymentsQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class FeedsPaymentsProfile : Profile
{
    public FeedsPaymentsProfile()
    {
        CreateMap<FeedPaymentEntity, FeedPaymentRowDto>()
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year));
    }
}