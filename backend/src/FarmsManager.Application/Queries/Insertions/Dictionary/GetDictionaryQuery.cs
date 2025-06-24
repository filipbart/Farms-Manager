using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Queries.Hatcheries;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Insertions.Dictionary;

public class GetDictionaryQuery : IRequest<BaseResponse<GetDictionaryQueryResponse>>;

public class GetDictionaryQueryHandler : IRequestHandler<GetDictionaryQuery, BaseResponse<GetDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly ICycleRepository _cycleRepository;

    public GetDictionaryQueryHandler(IFarmRepository farmRepository, IHatcheryRepository hatcheryRepository,
        ICycleRepository cycleRepository)
    {
        _farmRepository = farmRepository;
        _hatcheryRepository = hatcheryRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<BaseResponse<GetDictionaryQueryResponse>> Handle(GetDictionaryQuery request, CancellationToken ct)
    {
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(), ct);
        var hatcheries = await _hatcheryRepository.ListAsync<HatcheryDictModel>(new GetAllHatcheriesSpec(), ct);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), ct);


        return BaseResponse.CreateResponse(new GetDictionaryQueryResponse
        {
            Farms = farms,
            Hatcheries = hatcheries,
            Cycles = cycles
        });
    }
}

public sealed class GetAllCyclesSpec : BaseSpecification<CycleEntity>
{
    public GetAllCyclesSpec()
    {
        EnsureExists();
    }
}