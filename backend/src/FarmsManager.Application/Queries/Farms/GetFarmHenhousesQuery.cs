using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Domain.Models.FarmAggregate;
using MediatR;

namespace FarmsManager.Application.Queries.Farms;

public record GetFarmHenhousesQuery(Guid FarmId) : IRequest<BaseResponse<GetFarmHenhousesQueryResponse>>;

public class GetFarmHenhousesQueryResponse : PaginationModel<HenhouseRowDto>;

public class
    GetFarmHenhousesQueryHandler : IRequestHandler<GetFarmHenhousesQuery, BaseResponse<GetFarmHenhousesQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetFarmHenhousesQueryHandler(IFarmRepository farmRepository, IHenhouseRepository henhouseRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _farmRepository = farmRepository;
        _henhouseRepository = henhouseRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetFarmHenhousesQueryResponse>> Handle(GetFarmHenhousesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var isAdmin = user.IsAdmin;

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        var henhouses =
            await _henhouseRepository.ListAsync<HenhouseRowDto>(new HenhousesByFarmIdSpec(farm.Id, isAdmin), cancellationToken);

        return BaseResponse.CreateResponse(new GetFarmHenhousesQueryResponse
        {
            TotalRows = henhouses.Count,
            Items = henhouses.ClearAdminData(isAdmin)
        });
    }
}