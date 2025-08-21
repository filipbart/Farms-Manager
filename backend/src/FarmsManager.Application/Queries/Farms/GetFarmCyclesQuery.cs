using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Models.FarmAggregate;
using MediatR;

namespace FarmsManager.Application.Queries.Farms;

public record GetFarmCyclesQuery(Guid FarmId) : IRequest<BaseResponse<List<CycleDto>>>;

public class
    GetFarmCyclesQueryHandler : IRequestHandler<GetFarmCyclesQuery, BaseResponse<List<CycleDto>>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;

    public GetFarmCyclesQueryHandler(IFarmRepository farmRepository, ICycleRepository cycleRepository)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<BaseResponse<List<CycleDto>>> Handle(GetFarmCyclesQuery request,
        CancellationToken cancellationToken)
    {
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        var cycles = await _cycleRepository.ListAsync<CycleDto>(new GetCyclesByFarmIdSpec(farm.Id), cancellationToken);

        return BaseResponse.CreateResponse(cycles);
    }
}

public sealed class GetCyclesByFarmIdSpec : BaseSpecification<CycleEntity>
{
    public GetCyclesByFarmIdSpec(Guid farmId)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.FarmId == farmId);
        Query.OrderBy(t => t.Year).ThenBy(t => t.Identifier);
    }
}