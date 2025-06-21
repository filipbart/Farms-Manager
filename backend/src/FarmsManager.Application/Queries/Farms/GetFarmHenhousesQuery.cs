using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Models.FarmAggregate;
using MediatR;

namespace FarmsManager.Application.Queries.Farms;

public record GetFarmHenhousesQuery(Guid FarmId) : IRequest<BaseResponse<GetFarmHenhousesQueryResponse>>;

public class GetFarmHenhousesQueryResponse : PaginationModel<HenhouseRowDto>;

public class
    GetFarmHenhousesQueryHandler : IRequestHandler<GetFarmHenhousesQuery, BaseResponse<GetFarmHenhousesQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IHenhouseRepository _henhouseRepository;

    public GetFarmHenhousesQueryHandler(IFarmRepository farmRepository, IHenhouseRepository henhouseRepository)
    {
        _farmRepository = farmRepository;
        _henhouseRepository = henhouseRepository;
    }

    public async Task<BaseResponse<GetFarmHenhousesQueryResponse>> Handle(GetFarmHenhousesQuery request,
        CancellationToken cancellationToken)
    {
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        var henhouses =
            await _henhouseRepository.ListAsync<HenhouseRowDto>(new HenhousesByFarmIdSpec(farm.Id), cancellationToken);

        return BaseResponse.CreateResponse(new GetFarmHenhousesQueryResponse
        {
            TotalRows = henhouses.Count,
            Items = henhouses
        });
    }
}