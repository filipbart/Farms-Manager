using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Dashboard;

public class GetDashboardDictionaryQuery : IRequest<BaseResponse<GetDashboardDictionaryQueryResponse>>;

public class GetDashboardDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public class GetDashboardDictionaryQueryHandler : IRequestHandler<GetDashboardDictionaryQuery,
    BaseResponse<GetDashboardDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetDashboardDictionaryQueryHandler(IFarmRepository farmRepository, ICycleRepository cycleRepository,
        IUserRepository userRepository, IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<GetDashboardDictionaryQueryResponse>> Handle(GetDashboardDictionaryQuery request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        var accessibleFarmIds = user.IsAdmin ? null : user.Farms?.Select(t => t.FarmId).ToList();

        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(accessibleFarmIds), ct);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), ct);

        return BaseResponse.CreateResponse(new GetDashboardDictionaryQueryResponse
        {
            Farms = farms,
            Cycles = cycles
        });
    }
}