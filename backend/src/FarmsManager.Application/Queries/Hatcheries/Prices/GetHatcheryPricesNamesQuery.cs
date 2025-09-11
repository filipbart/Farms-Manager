using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Hatcheries.Prices;

public record GetHatcheryPricesNamesQueryResponse
{
    public List<DictModel> Hatcheries { get; set; } = [];
}

public record GetHatcheryPricesNamesQuery : IRequest<BaseResponse<GetHatcheryPricesNamesQueryResponse>>;

public class GetHatcheryPricesNamesQueryHandler : IRequestHandler<GetHatcheryPricesNamesQuery,
    BaseResponse<GetHatcheryPricesNamesQueryResponse>>
{
    private readonly IHatcheryNameRepository _hatcheryNameRepository;

    public GetHatcheryPricesNamesQueryHandler(IHatcheryNameRepository hatcheryNameRepository)
    {
        _hatcheryNameRepository = hatcheryNameRepository;
    }

    public async Task<BaseResponse<GetHatcheryPricesNamesQueryResponse>> Handle(
        GetHatcheryPricesNamesQuery request, CancellationToken cancellationToken)
    {
        var hatcheries =
            await _hatcheryNameRepository.ListAsync<DictModel>(new GetAllHatcheriesNamesSpec(), cancellationToken);

        return BaseResponse.CreateResponse(new GetHatcheryPricesNamesQueryResponse
        {
            Hatcheries = hatcheries,
        });
    }
}

public sealed class GetAllHatcheriesNamesSpec : BaseSpecification<HatcheryNameEntity>
{
    public GetAllHatcheriesNamesSpec()
    {
        EnsureExists();
        DisableTracking();
    }
}