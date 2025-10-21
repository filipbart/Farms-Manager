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

namespace FarmsManager.Application.Queries.Gas.Consumptions;

public record GetGasConsumptionsDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public record GetGasConsumptionsDictionaryQuery : IRequest<BaseResponse<GetGasConsumptionsDictionaryQueryResponse>>;

public class GetGasConsumptionsDictionaryQueryHandler : IRequestHandler<GetGasConsumptionsDictionaryQuery,
    BaseResponse<GetGasConsumptionsDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetGasConsumptionsDictionaryQueryHandler(IFarmRepository farmRepository,
        ICycleRepository cycleRepository, IUserRepository userRepository, IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<GetGasConsumptionsDictionaryQueryResponse>> Handle(
        GetGasConsumptionsDictionaryQuery request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(accessibleFarmIds, user.IsAdmin),
            cancellationToken);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), cancellationToken);

        return BaseResponse.CreateResponse(new GetGasConsumptionsDictionaryQueryResponse
        {
            Farms = farms,
            Cycles = cycles
        });
    }
}