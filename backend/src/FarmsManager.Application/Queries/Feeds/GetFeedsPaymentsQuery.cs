using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Feeds;

public enum FeedsPaymentsOrderBy
{
    DateCreatedUtc,
}

public record GetFeedsPaymentsQueryFilters : OrderedPaginationParams<FeedsPaymentsOrderBy>;

public record GetFeedsPaymentsQuery(GetFeedsPaymentsQueryFilters Filters)
    : IRequest<BaseResponse<GetFeedsPaymentsQueryResponse>>;

public record FeedPaymentRowDto
{
    public Guid Id { get; init; }
    public string FarmName { get; init; }
    public string FilePath { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetFeedsPaymentsQueryResponse : PaginationModel<FeedPaymentRowDto>;

public class
    GetFeedsPaymentsQueryHandler : IRequestHandler<GetFeedsPaymentsQuery,
    BaseResponse<GetFeedsPaymentsQueryResponse>>
{
    private readonly IFeedPaymentRepository _feedPaymentRepository;

    public GetFeedsPaymentsQueryHandler(IFeedPaymentRepository feedPaymentRepository)
    {
        _feedPaymentRepository = feedPaymentRepository;
    }

    public async Task<BaseResponse<GetFeedsPaymentsQueryResponse>> Handle(GetFeedsPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _feedPaymentRepository.ListAsync<FeedPaymentRowDto>(
            new GetAllFeedsPaymentsSpec(request.Filters, true), cancellationToken);
        var count = await _feedPaymentRepository.CountAsync(
            new GetAllFeedsPaymentsSpec(request.Filters, false),
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
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name));
    }
}