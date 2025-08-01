using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Hatcheries.Prices;

public record GetHatcheryPricesDictionaryQueryResponse
{
    public List<DictModel> Hatcheries { get; set; } = [];
}

public record GetHatcheryPricesDictionaryQuery : IRequest<BaseResponse<GetHatcheryPricesDictionaryQueryResponse>>;

public class GetHatcheryPricesDictionaryQueryHandler : IRequestHandler<GetHatcheryPricesDictionaryQuery,
    BaseResponse<GetHatcheryPricesDictionaryQueryResponse>>
{
    private readonly IHatcheryRepository _hatcheryRepository;

    public GetHatcheryPricesDictionaryQueryHandler(IHatcheryRepository hatcheryRepository)
    {
        _hatcheryRepository = hatcheryRepository;
    }

    public async Task<BaseResponse<GetHatcheryPricesDictionaryQueryResponse>> Handle(
        GetHatcheryPricesDictionaryQuery request, CancellationToken cancellationToken)
    {
        var hatcheries = await _hatcheryRepository.ListAsync<DictModel>(new GetAllHatcheriesSpec(), cancellationToken);

        return BaseResponse.CreateResponse(new GetHatcheryPricesDictionaryQueryResponse
        {
            Hatcheries = hatcheries,
        });
    }
}