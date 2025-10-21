using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.FlockLosses;

public class GetProductionDataFlockLossesQueryHandler : IRequestHandler<GetProductionDataFlockLossesQuery,
    BaseResponse<GetProductionDataFlockLossesQueryResponse>>
{
    private readonly IProductionDataFlockLossMeasureRepository _flockLossRepository;
    private readonly IMapper _mapper;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetProductionDataFlockLossesQueryHandler(
        IProductionDataFlockLossMeasureRepository flockLossRepository,
        IMapper mapper, IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _flockLossRepository = flockLossRepository;
        _mapper = mapper;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetProductionDataFlockLossesQueryResponse>> Handle(
        GetProductionDataFlockLossesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;
        var isAdmin = user.IsAdmin;

        var spec = new GetAllProductionDataFlockLossesSpec(request.Filters, true, accessibleFarmIds, isAdmin);
        var countSpec = new GetAllProductionDataFlockLossesSpec(request.Filters, false, accessibleFarmIds, isAdmin);

        var flockLosses = await _flockLossRepository.ListAsync(spec, cancellationToken);
        var totalRows = await _flockLossRepository.CountAsync(countSpec, cancellationToken);

        var data = _mapper.Map<List<ProductionDataFlockLossRowDto>>(flockLosses);

        return BaseResponse.CreateResponse(new GetProductionDataFlockLossesQueryResponse
        {
            TotalRows = totalRows,
            Items = data.ClearAdminData(isAdmin)
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
                    : (decimal?)null))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.Name : null))
            .ForMember(dest => dest.ModifiedByName, opt => opt.MapFrom(src => src.Modifier != null ? src.Modifier.Name : null))
            .ForMember(dest => dest.DeletedByName, opt => opt.MapFrom(src => src.Deleter != null ? src.Deleter.Name : null));
    }
}