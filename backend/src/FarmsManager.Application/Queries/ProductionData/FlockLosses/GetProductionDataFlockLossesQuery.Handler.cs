using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.FlockLosses;

public class GetProductionDataFlockLossesQueryHandler : IRequestHandler<GetProductionDataFlockLossesQuery,
    BaseResponse<GetProductionDataFlockLossesQueryResponse>>
{
    private readonly IProductionDataFlockLossMeasureRepository _flockLossRepository;
    private readonly IMapper _mapper;

    public GetProductionDataFlockLossesQueryHandler(
        IProductionDataFlockLossMeasureRepository flockLossRepository,
        IMapper mapper)
    {
        _flockLossRepository = flockLossRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetProductionDataFlockLossesQueryResponse>> Handle(
        GetProductionDataFlockLossesQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new GetAllProductionDataFlockLossesSpec(request.Filters, true);
        var countSpec = new GetAllProductionDataFlockLossesSpec(request.Filters, false);

        var flockLosses = await _flockLossRepository.ListAsync(spec, cancellationToken);
        var totalRows = await _flockLossRepository.CountAsync(countSpec, cancellationToken);

        var data = _mapper.Map<List<ProductionDataFlockLossRowDto>>(flockLosses);

        return BaseResponse.CreateResponse(new GetProductionDataFlockLossesQueryResponse
        {
            TotalRows = totalRows,
            Items = data
        });
    }
}

public class ProductionDataFlockLossProfile : Profile
{
    public ProductionDataFlockLossProfile()
    {
        CreateMap<ProductionDataFlockLossMeasureEntity, ProductionDataFlockLossRowDto>()
            .ForMember(dest => dest.CycleText, opt => opt.MapFrom(src => src.Cycle.Identifier + "/" + src.Cycle.Year))
            .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name))
            .ForMember(dest => dest.HenhouseName, opt => opt.MapFrom(src => src.Henhouse.Name))
            .ForMember(dest => dest.HatcheryName, opt => opt.MapFrom(src => src.Hatchery.Name))
            .ForMember(dest => dest.InsertionQuantity, opt => opt.MapFrom(src => src.Insertion.Quantity))
            .ForMember(dest => dest.FlockLoss1Percentage, opt => opt.MapFrom(src =>
                src.FlockLoss1Quantity.HasValue && src.Insertion.Quantity > 0
                    ? (decimal)src.FlockLoss1Quantity.Value / src.Insertion.Quantity * 100
                    : (decimal?)null))
            .ForMember(dest => dest.FlockLoss2Percentage, opt => opt.MapFrom(src =>
                src.FlockLoss2Quantity.HasValue && src.Insertion.Quantity > 0
                    ? (decimal)src.FlockLoss2Quantity.Value / src.Insertion.Quantity * 100
                    : (decimal?)null))
            .ForMember(dest => dest.FlockLoss3Percentage, opt => opt.MapFrom(src =>
                src.FlockLoss3Quantity.HasValue && src.Insertion.Quantity > 0
                    ? (decimal)src.FlockLoss3Quantity.Value / src.Insertion.Quantity * 100
                    : (decimal?)null))
            .ForMember(dest => dest.FlockLoss4Percentage, opt => opt.MapFrom(src =>
                src.FlockLoss4Quantity.HasValue && src.Insertion.Quantity > 0
                    ? (decimal)src.FlockLoss4Quantity.Value / src.Insertion.Quantity * 100
                    : (decimal?)null));
    }
}