using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Queries.Hatcheries;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Summary;

public class GetSummaryDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<DictModel> Hatcheries { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public class GetSummaryDictionaryQuery : IRequest<BaseResponse<GetSummaryDictionaryQueryResponse>>;

public class GetSummaryDictionaryQueryHandler : IRequestHandler<GetSummaryDictionaryQuery,
    BaseResponse<GetSummaryDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetSummaryDictionaryQueryHandler(IFarmRepository farmRepository, IHatcheryRepository hatcheryRepository,
        ICycleRepository cycleRepository, IUserRepository userRepository, IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _hatcheryRepository = hatcheryRepository;
        _cycleRepository = cycleRepository;
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<GetSummaryDictionaryQueryResponse>> Handle(GetSummaryDictionaryQuery request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(accessibleFarmIds, user.IsAdmin), ct);
        var hatcheries = await _hatcheryRepository.ListAsync<DictModel>(new GetAllHatcheriesSpec(user.IsAdmin), ct);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), ct);


        return BaseResponse.CreateResponse(new GetSummaryDictionaryQueryResponse
        {
            Farms = farms,
            Hatcheries = hatcheries,
            Cycles = cycles
        });
    }
}