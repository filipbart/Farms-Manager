using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Models.FarmAggregate;
using MediatR;

namespace FarmsManager.Application.Queries.Farms;

public record GetFarmLatestCycleQuery(Guid FarmId) : IRequest<BaseResponse<CycleDto>>;

public class
    GetFarmLatestCycleQueryHandler : IRequestHandler<GetFarmLatestCycleQuery, BaseResponse<CycleDto>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IMapper _mapper;

    public GetFarmLatestCycleQueryHandler(IFarmRepository farmRepository, IMapper mapper)
    {
        _farmRepository = farmRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<CycleDto>> Handle(GetFarmLatestCycleQuery request,
        CancellationToken cancellationToken)
    {
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        var activeCycle = farm.ActiveCycle;

        var response = activeCycle != null ? _mapper.Map<CycleDto>(activeCycle) : null;
        return BaseResponse.CreateResponse(response);
    }
}