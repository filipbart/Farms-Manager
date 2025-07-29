using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.TransferFeed;

public class GetProductionDataTransferFeedQueryHandler : IRequestHandler<GetProductionDataTransferFeedQuery,
    BaseResponse<GetProductionDataTransferFeedQueryResponse>>
{
    private readonly IProductionDataTransferFeedRepository _repository;

    public GetProductionDataTransferFeedQueryHandler(IProductionDataTransferFeedRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseResponse<GetProductionDataTransferFeedQueryResponse>> Handle(
        GetProductionDataTransferFeedQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _repository.ListAsync<ProductionDataTransferFeedRowDto>(
            new GetAllProductionDataTransferFeedSpec(request.Filters, true), cancellationToken);
        var count = await _repository.CountAsync(
            new GetAllProductionDataTransferFeedSpec(request.Filters, false),
            cancellationToken);

        return BaseResponse.CreateResponse(new GetProductionDataTransferFeedQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class ProductionDataTransferFeedProfile : Profile
{
    public ProductionDataTransferFeedProfile()
    {
        CreateMap<ProductionDataTransferFeedEntity, ProductionDataTransferFeedRowDto>()
            .ForMember(t => t.FromCycleText, opt => opt.MapFrom(t => t.FromCycle.Identifier + "/" + t.FromCycle.Year))
            .ForMember(t => t.FromFarmName, opt => opt.MapFrom(t => t.FromFarm.Name))
            .ForMember(t => t.FromHenhouseName, opt => opt.MapFrom(t => t.FromHenhouse.Name))
            .ForMember(t => t.ToCycleText, opt => opt.MapFrom(t => t.ToCycle.Identifier + "/" + t.ToCycle.Year))
            .ForMember(t => t.ToFarmName, opt => opt.MapFrom(t => t.ToFarm.Name))
            .ForMember(t => t.ToHenhouseName, opt => opt.MapFrom(t => t.ToHenhouse.Name))
            .ForMember(t => t.FeedName, opt => opt.MapFrom(t => t.FeedName));
    }
}