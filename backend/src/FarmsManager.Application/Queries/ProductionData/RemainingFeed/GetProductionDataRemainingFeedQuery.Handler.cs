using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.RemainingFeed;

public class GetProductionDataRemainingFeedQueryHandler : IRequestHandler<GetProductionDataRemainingFeedQuery,
    BaseResponse<GetProductionDataRemainingFeedQueryResponse>>
{
    private readonly IProductionDataRemainingFeedRepository _repository;

    public GetProductionDataRemainingFeedQueryHandler(IProductionDataRemainingFeedRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseResponse<GetProductionDataRemainingFeedQueryResponse>> Handle(
        GetProductionDataRemainingFeedQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _repository.ListAsync<ProductionDataRemainingFeedRowDto>(
            new GetAllProductionDataRemainingFeedSpec(request.Filters, true), cancellationToken);
        var count = await _repository.CountAsync(
            new GetAllProductionDataRemainingFeedSpec(request.Filters, false),
            cancellationToken);

        return BaseResponse.CreateResponse(new GetProductionDataRemainingFeedQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class ProductionDataRemainingFeedProfile : Profile
{
    public ProductionDataRemainingFeedProfile()
    {
        CreateMap<ProductionDataRemainingFeedEntity, ProductionDataRemainingFeedRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name))
            .ForMember(t => t.FeedName, opt => opt.MapFrom(t => t.FeedName));
    }
}