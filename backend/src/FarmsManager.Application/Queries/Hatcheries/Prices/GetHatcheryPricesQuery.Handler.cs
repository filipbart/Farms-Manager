using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Hatcheries.Prices;

public class GetHatcheryPricesQueryHandler : IRequestHandler<GetHatcheryPricesQuery,
    BaseResponse<GetHatcheryPricesQueryResponse>>
{
    private readonly IHatcheryPriceRepository _hatcheryPriceRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetHatcheryPricesQueryHandler(IHatcheryPriceRepository hatcheryPriceRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _hatcheryPriceRepository = hatcheryPriceRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetHatcheryPricesQueryResponse>> Handle(GetHatcheryPricesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var isAdmin = user.IsAdmin;

        var data = await _hatcheryPriceRepository.ListAsync<HatcheryPriceRowDto>(
            new GetAllHatcheryPricesSpec(request.Filters, true, isAdmin), cancellationToken);

        var count = await _hatcheryPriceRepository.CountAsync(
            new GetAllHatcheryPricesSpec(request.Filters, false, isAdmin), cancellationToken);

        return BaseResponse.CreateResponse(new GetHatcheryPricesQueryResponse
        {
            TotalRows = count,
            Items = data.ClearAdminData(isAdmin)
        });
    }
}

public class HatcheryPriceProfile : Profile
{
    public HatcheryPriceProfile()
    {
        CreateMap<HatcheryPriceEntity, HatcheryPriceRowDto>()
            .ForMember(m => m.CreatedByName, opt => opt.MapFrom(t => t.Creator != null ? t.Creator.Name : null))
            .ForMember(m => m.ModifiedByName, opt => opt.MapFrom(t => t.Modifier != null ? t.Modifier.Name : null))
            .ForMember(m => m.DeletedByName, opt => opt.MapFrom(t => t.Deleter != null ? t.Deleter.Name : null));
    }
}