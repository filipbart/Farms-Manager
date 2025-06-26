using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Models.FarmAggregate;
using MediatR;

namespace FarmsManager.Application.Queries.Insertions.Henhouses;

public record GetAvailableHenhousesQuery(Guid FarmId)
    : IRequest<BaseResponse<GetAvailableHenhousesQueryResponse>>;

public record GetAvailableHenhousesQueryResponse
{
    public List<HenhouseRowDto> Items { get; init; }
}

public class GetAvailableHenhousesQueryHandler : IRequestHandler<GetAvailableHenhousesQuery,
    BaseResponse<GetAvailableHenhousesQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IInsertionRepository _insertionRepository;

    public GetAvailableHenhousesQueryHandler(IFarmRepository farmRepository, IHenhouseRepository henhouseRepository,
        IInsertionRepository insertionRepository)
    {
        _farmRepository = farmRepository;
        _henhouseRepository = henhouseRepository;
        _insertionRepository = insertionRepository;
    }

    public async Task<BaseResponse<GetAvailableHenhousesQueryResponse>> Handle(GetAvailableHenhousesQuery request,
        CancellationToken cancellationToken)
    {
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        var henhouses =
            await _henhouseRepository.ListAsync<HenhouseRowDto>(new HenhousesByFarmIdSpec(farm.Id), cancellationToken);

        var insertionHenhouses = await _insertionRepository.ListAsync(
            new GetInsertionHenhousesForCycleAndFarmIdSpec(farm.Id, farm.ActiveCycle.Id), cancellationToken);

        var result = henhouses.Where(t => !insertionHenhouses.Contains(t.Id)).ToList();

        return BaseResponse.CreateResponse(new GetAvailableHenhousesQueryResponse
        {
            Items = result
        });
    }
}

public sealed class GetInsertionHenhousesForCycleAndFarmIdSpec : BaseSpecification<InsertionEntity, Guid>
{
    public GetInsertionHenhousesForCycleAndFarmIdSpec(Guid farmId, Guid cycleId)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.CycleId == cycleId);
        Query.Where(t => t.FarmId == farmId);
        Query.Select(t => t.HenhouseId);
    }
}