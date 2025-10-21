using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Gas.Deliveries;

public class GetGasDeliveriesQueryHandler : IRequestHandler<GetGasDeliveriesQuery,
    BaseResponse<GetGasDeliveriesQueryResponse>>
{
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetGasDeliveriesQueryHandler(IGasDeliveryRepository gasDeliveryRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _gasDeliveryRepository = gasDeliveryRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetGasDeliveriesQueryResponse>> Handle(GetGasDeliveriesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;
        var isAdmin = user.IsAdmin;

        var data = await _gasDeliveryRepository.ListAsync<GasDeliveryRowDto>(
            new GetAllGasDeliveriesSpec(request.Filters, true, accessibleFarmIds, isAdmin), cancellationToken);

        var count = await _gasDeliveryRepository.CountAsync(
            new GetAllGasDeliveriesSpec(request.Filters, false, accessibleFarmIds, isAdmin), cancellationToken);

        return BaseResponse.CreateResponse(new GetGasDeliveriesQueryResponse
        {
            TotalRows = count,
            Items = data.ClearAdminData(isAdmin)
        });
    }
}

public class GasDeliveryProfile : Profile
{
    public GasDeliveryProfile()
    {
        CreateMap<GasDeliveryEntity, GasDeliveryRowDto>()
            .ForMember(m => m.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(m => m.ContractorName, opt => opt.MapFrom(t => t.GasContractor.Name))
            .ForMember(m => m.CreatedByName, opt => opt.MapFrom(t => t.Creator != null ? t.Creator.Name : null))
            .ForMember(m => m.ModifiedByName, opt => opt.MapFrom(t => t.Modifier != null ? t.Modifier.Name : null))
            .ForMember(m => m.DeletedByName, opt => opt.MapFrom(t => t.Deleter != null ? t.Deleter.Name : null));
    }
}