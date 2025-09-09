using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Failures;

public class GetProductionDataFailuresQueryHandler : IRequestHandler<GetProductionDataFailuresQuery,
    BaseResponse<GetProductionDataFailuresQueryResponse>>
{
    private readonly IProductionDataFailureRepository _productionDataFailureRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetProductionDataFailuresQueryHandler(IProductionDataFailureRepository productionDataFailureRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _productionDataFailureRepository = productionDataFailureRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetProductionDataFailuresQueryResponse>> Handle(
        GetProductionDataFailuresQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var data = await _productionDataFailureRepository.ListAsync<ProductionDataFailureRowDto>(
            new GetAllProductionDataFailuresSpec(request.Filters, true, accessibleFarmIds), cancellationToken);
        var count = await _productionDataFailureRepository.CountAsync(
            new GetAllProductionDataFailuresSpec(request.Filters, false, accessibleFarmIds),
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