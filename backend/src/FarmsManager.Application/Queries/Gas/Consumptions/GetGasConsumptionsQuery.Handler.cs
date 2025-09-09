using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Gas.Consumptions;

public class GetGasConsumptionsQueryHandler : IRequestHandler<GetGasConsumptionsQuery,
    BaseResponse<GetGasConsumptionsQueryResponse>>
{
    private readonly IGasConsumptionRepository _gasConsumptionRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetGasConsumptionsQueryHandler(IGasConsumptionRepository gasConsumptionRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _gasConsumptionRepository = gasConsumptionRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetGasConsumptionsQueryResponse>> Handle(GetGasConsumptionsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var data = await _gasConsumptionRepository.ListAsync<GasConsumptionRowDto>(
            new GetAllGasConsumptionsSpec(request.Filters, true, accessibleFarmIds), cancellationToken);

        var count = await _gasConsumptionRepository.CountAsync(
            new GetAllGasConsumptionsSpec(request.Filters, false, accessibleFarmIds), cancellationToken);

        return BaseResponse.CreateResponse(new GetGasConsumptionsQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class GasConsumptionProfile : Profile
{
    public GasConsumptionProfile()
    {
        CreateMap<GasConsumptionEntity, GasConsumptionRowDto>()
            .ForMember(m => m.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(m => m.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year));
    }
}