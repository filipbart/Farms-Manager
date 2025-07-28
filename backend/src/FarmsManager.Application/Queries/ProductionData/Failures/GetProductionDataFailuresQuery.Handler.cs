using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Failures;

public class GetProductionDataFailuresQueryHandler : IRequestHandler<GetProductionDataFailuresQuery,
    BaseResponse<GetProductionDataFailuresQueryResponse>>
{
    private readonly IProductionDataFailureRepository _productionDataFailureRepository;

    public GetProductionDataFailuresQueryHandler(IProductionDataFailureRepository productionDataFailureRepository)
    {
        _productionDataFailureRepository = productionDataFailureRepository;
    }

    public async Task<BaseResponse<GetProductionDataFailuresQueryResponse>> Handle(
        GetProductionDataFailuresQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _productionDataFailureRepository.ListAsync<ProductionDataFailureRowDto>(
            new GetAllProductionDataFailuresSpec(request.Filters, true), cancellationToken);
        var count = await _productionDataFailureRepository.CountAsync(
            new GetAllProductionDataFailuresSpec(request.Filters, false),
            cancellationToken);

        return BaseResponse.CreateResponse(new GetProductionDataFailuresQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class ProductionDataFailureProfile : Profile
{
    public ProductionDataFailureProfile()
    {
        CreateMap<ProductionDataFailureEntity, ProductionDataFailureRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name));
    }
}