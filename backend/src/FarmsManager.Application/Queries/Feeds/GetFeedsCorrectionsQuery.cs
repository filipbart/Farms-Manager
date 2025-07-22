using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Feeds;

public enum FeedsCorrectionsOrderBy
{
    DateCreatedUtc,
}

public record GetFeedsCorrectionsQueryFilters : OrderedPaginationParams<FeedsCorrectionsOrderBy>;

public record GetFeedsCorrectionsQuery(GetFeedsCorrectionsQueryFilters Filters)
    : IRequest<BaseResponse<GetFeedsCorrectionsQueryResponse>>;

public record FeedCorrectionRowDto
{
    public Guid Id { get; init; }
    public string FarmName { get; init; }
    public string InvoiceNumber { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetFeedsCorrectionsQueryResponse : PaginationModel<FeedCorrectionRowDto>;

public class
    GetFeedsCorrectionsQueryHandler : IRequestHandler<GetFeedsCorrectionsQuery,
    BaseResponse<GetFeedsCorrectionsQueryResponse>>
{
    private readonly IFeedInvoiceCorrectionRepository _feedInvoiceCorrectionRepository;

    public GetFeedsCorrectionsQueryHandler(IFeedInvoiceCorrectionRepository feedInvoiceCorrectionRepository)
    {
        _feedInvoiceCorrectionRepository = feedInvoiceCorrectionRepository;
    }

    public async Task<BaseResponse<GetFeedsCorrectionsQueryResponse>> Handle(GetFeedsCorrectionsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _feedInvoiceCorrectionRepository.ListAsync<FeedCorrectionRowDto>(
            new GetAllFeedsCorrectionsSpec(request.Filters, true), cancellationToken);
        var count = await _feedInvoiceCorrectionRepository.CountAsync(
            new GetAllFeedsCorrectionsSpec(request.Filters, false),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetFeedsCorrectionsQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class FeedsCorrectionsProfile : Profile
{
    public FeedsCorrectionsProfile()
    {
        CreateMap<FeedInvoiceCorrectionEntity, FeedCorrectionRowDto>()
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name));
    }
}