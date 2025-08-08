using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.FallenStock;

public class GetFallenStocksDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public class GetFallenStocksDictionaryQuery : IRequest<BaseResponse<GetFallenStocksDictionaryQueryResponse>>;

public class GetFallenStocksDictionaryQueryHandler : IRequestHandler<GetFallenStocksDictionaryQuery,
    BaseResponse<GetFallenStocksDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;

    public GetFallenStocksDictionaryQueryHandler(IFarmRepository farmRepository, ICycleRepository cycleRepository)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<BaseResponse<GetFallenStocksDictionaryQueryResponse>> Handle(
        GetFallenStocksDictionaryQuery request,
        CancellationToken ct)
    {
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(), ct);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), ct);


        return BaseResponse.CreateResponse(new GetFallenStocksDictionaryQueryResponse
        {
            Farms = farms,
            Cycles = cycles
        });
    }
}