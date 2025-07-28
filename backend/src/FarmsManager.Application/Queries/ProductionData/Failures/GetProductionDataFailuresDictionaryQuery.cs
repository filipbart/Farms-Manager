using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Failures;

public class
    GetProductionDataFailuresDictionaryQuery : IRequest<BaseResponse<GetProductionDataFailuresDictionaryQueryResponse>>;

public class GetProductionDataFailuresDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public class GetProductionDataFailuresDictionaryQueryHandler : IRequestHandler<GetProductionDataFailuresDictionaryQuery,
    BaseResponse<GetProductionDataFailuresDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;

    public GetProductionDataFailuresDictionaryQueryHandler(IFarmRepository farmRepository,
        ICycleRepository cycleRepository)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<BaseResponse<GetProductionDataFailuresDictionaryQueryResponse>> Handle(
        GetProductionDataFailuresDictionaryQuery request,
        CancellationToken ct)
    {
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(), ct);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), ct);

        return BaseResponse.CreateResponse(new GetProductionDataFailuresDictionaryQueryResponse
        {
            Farms = farms,
            Cycles = cycles
        });
    }
}