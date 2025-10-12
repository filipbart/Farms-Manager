using MediatR;
using FarmsManager.Application.Common.Responses;
using AutoMapper;
using FarmsManager.Application.Queries.FallenStock;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Models.FarmAggregate;

namespace FarmsManager.Application.Queries.Farms;

public record GetInsertedHenhousesQuery(Guid FarmId, Guid CycleId) : IRequest<BaseResponse<List<HenhouseRowDto>>>;

public class
    GetInsertedHenhousesQueryHandler : IRequestHandler<GetInsertedHenhousesQuery,
    BaseResponse<List<HenhouseRowDto>>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IInsertionRepository _insertionRepository;
    private readonly IMapper _mapper;

    public GetInsertedHenhousesQueryHandler(IFarmRepository farmRepository, ICycleRepository cycleRepository,
        IHenhouseRepository henhouseRepository, IInsertionRepository insertionRepository, IMapper mapper)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _insertionRepository = insertionRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<HenhouseRowDto>>> Handle(GetInsertedHenhousesQuery request,
        CancellationToken cancellationToken)
    {
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), cancellationToken);

        var henhouses =
            await _henhouseRepository.ListAsync(new HenhousesByFarmIdSpec(request.FarmId), cancellationToken);

        var insertions = await _insertionRepository.ListAsync(new GetInsertionsByFarmAndCycleSpec(farm.Id, cycle.Id),
            cancellationToken);

        var henhouseIdsWithInsertions = insertions.Select(i => i.HenhouseId).Distinct().ToList();
        var filteredHenhouses = henhouses.Where(h => henhouseIdsWithInsertions.Contains(h.Id)).ToList();


        var result = _mapper.Map<List<HenhouseRowDto>>(filteredHenhouses);

        return new BaseResponse<List<HenhouseRowDto>>(result);
    }
}