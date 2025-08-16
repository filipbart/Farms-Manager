using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Queries.Slaughterhouses;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Sales.Dictionary;

public class GetSalesDictionaryQuery : IRequest<BaseResponse<GetSalesDictionaryQueryResponse>>;

public class GetSalesDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<DictModel> Slaughterhouses { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public class GetSalesDictionaryQueryHandler : IRequestHandler<GetSalesDictionaryQuery,
    BaseResponse<GetSalesDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetSalesDictionaryQueryHandler(IFarmRepository farmRepository, ICycleRepository cycleRepository,
        ISlaughterhouseRepository slaughterhouseRepository, IUserRepository userRepository,
        IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _slaughterhouseRepository = slaughterhouseRepository;
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
    }


    public async Task<BaseResponse<GetSalesDictionaryQueryResponse>> Handle(GetSalesDictionaryQuery request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        var accessibleFarmIds = user.Farms?.Select(t => t.FarmId).ToList();

        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(accessibleFarmIds), ct);
        var slaughterhouses = await _slaughterhouseRepository.ListAsync<DictModel>(new GetAllSlaughterhousesSpec(), ct);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), ct);


        return BaseResponse.CreateResponse(new GetSalesDictionaryQueryResponse
        {
            Farms = farms,
            Slaughterhouses = slaughterhouses,
            Cycles = cycles
        });
    }
}