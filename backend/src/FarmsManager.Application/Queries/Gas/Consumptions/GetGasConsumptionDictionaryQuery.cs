using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
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

    public GetGasConsumptionsDictionaryQueryHandler(IFarmRepository farmRepository,
        ICycleRepository cycleRepository)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<BaseResponse<GetGasConsumptionsDictionaryQueryResponse>> Handle(
        GetGasConsumptionsDictionaryQuery request, CancellationToken cancellationToken)
    {
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(), cancellationToken);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), cancellationToken);

        return BaseResponse.CreateResponse(new GetGasConsumptionsDictionaryQueryResponse
        {
            Farms = farms,
            Cycles = cycles
        });
    }
}