using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.FallenStocks;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Models.FarmAggregate;
using MediatR;

namespace FarmsManager.Application.Queries.FallenStock;

public record GetAvailableHenhousesQueryResponse
{
    public List<HenhouseRowDto> Henhouses { get; set; } = [];
}

public record GetAvailableHenhousesQuery(Guid FarmId, Guid CycleId, DateOnly Date)
    : IRequest<BaseResponse<GetAvailableHenhousesQueryResponse>>;

public class GetAvailableHenhousesQueryHandler : IRequestHandler<GetAvailableHenhousesQuery,
    BaseResponse<GetAvailableHenhousesQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IFallenStockRepository _fallenStockRepository;
    private readonly IMapper _mapper;

    public GetAvailableHenhousesQueryHandler(IFarmRepository farmRepository, ICycleRepository cycleRepository,
        IHenhouseRepository henhouseRepository, IFallenStockRepository fallenStockRepository, IMapper mapper)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _fallenStockRepository = fallenStockRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetAvailableHenhousesQueryResponse>> Handle(GetAvailableHenhousesQuery request,
        CancellationToken ct)
    {
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);

        var henhouses = await _henhouseRepository.ListAsync(new HenhousesByFarmIdSpec(farm.Id), ct);

        var fallenStocks =
            await _fallenStockRepository.ListAsync(
                new GetFallenStocksByFarmCycleAndDateSpec(farm.Id, cycle.Id, request.Date), ct);

        var availableHenhouses = henhouses.Where(h => !fallenStocks.Select(f => f.HenhouseId).Contains(h.Id)).ToList();
        var mappedHenhouses = _mapper.Map<List<HenhouseRowDto>>(availableHenhouses);

        return BaseResponse.CreateResponse(new GetAvailableHenhousesQueryResponse
        {
            Henhouses = mappedHenhouses
        });
    }
}