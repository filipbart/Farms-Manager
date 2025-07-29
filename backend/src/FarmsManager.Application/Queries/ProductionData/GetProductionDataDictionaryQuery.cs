using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData;

public class GetProductionDataDictionaryQuery : IRequest<BaseResponse<GetProductionDataDictionaryQueryResponse>>;

public class GetProductionDataDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public class GetProductionDataDictionaryQueryHandler : IRequestHandler<GetProductionDataDictionaryQuery,
    BaseResponse<GetProductionDataDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;

    public GetProductionDataDictionaryQueryHandler(IFarmRepository farmRepository,
        ICycleRepository cycleRepository)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<BaseResponse<GetProductionDataDictionaryQueryResponse>> Handle(
        GetProductionDataDictionaryQuery request,
        CancellationToken ct)
    {
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(), ct);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), ct);

        return BaseResponse.CreateResponse(new GetProductionDataDictionaryQueryResponse
        {
            Farms = farms,
            Cycles = cycles
        });
    }
}