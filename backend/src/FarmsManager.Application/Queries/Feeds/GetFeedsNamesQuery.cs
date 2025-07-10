using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Feeds;

public record GetFeedsNamesQuery : IRequest<BaseResponse<GetFeedsNamesQueryResponse>>;

public record GetFeedsNamesQueryResponse
{
    public List<FeedNameRow> Fields { get; init; }
}

public record FeedNameRow
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}

public class GetFeedsNamesQueryHandler : IRequestHandler<GetFeedsNamesQuery, BaseResponse<GetFeedsNamesQueryResponse>>
{
    private readonly IFeedNameRepository _feedNameRepository;

    public GetFeedsNamesQueryHandler(IFeedNameRepository feedNameRepository)
    {
        _feedNameRepository = feedNameRepository;
    }

    public async Task<BaseResponse<GetFeedsNamesQueryResponse>> Handle(GetFeedsNamesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _feedNameRepository.ListAsync<FeedNameRow>(new GetAllFeedsNamesSpec(),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetFeedsNamesQueryResponse
        {
            Fields = items
        });
    }
}

public sealed class GetAllFeedsNamesSpec : BaseSpecification<FeedNameEntity>
{
    public GetAllFeedsNamesSpec()
    {
        EnsureExists();
    }
}

public class FeedsNamesProfile : Profile
{
    public FeedsNamesProfile()
    {
        CreateMap<FeedNameEntity, FeedNameRow>();
    }
}