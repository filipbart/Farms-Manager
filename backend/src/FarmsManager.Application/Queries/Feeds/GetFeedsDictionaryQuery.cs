using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Queries.Insertions.Dictionary;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Feeds;

public class GetFeedsDictionaryQuery : IRequest<BaseResponse<GetFeedsDictionaryQueryResponse>>;

public class GetFeedsDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public class GetFeedsDictionaryQueryHandler : IRequestHandler<GetFeedsDictionaryQuery,
    BaseResponse<GetFeedsDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;

    public GetFeedsDictionaryQueryHandler(IFarmRepository farmRepository, ICycleRepository cycleRepository)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
    }


    public async Task<BaseResponse<GetFeedsDictionaryQueryResponse>> Handle(GetFeedsDictionaryQuery request,
        CancellationToken ct)
    {
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(), ct);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), ct);


        return BaseResponse.CreateResponse(new GetFeedsDictionaryQueryResponse
        {
            Farms = farms,
            Cycles = cycles
        });
    }
}