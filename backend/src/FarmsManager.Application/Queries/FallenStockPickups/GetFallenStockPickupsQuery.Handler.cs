using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.FallenStock;
using FarmsManager.Application.Specifications.FallenStocks;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.FallenStockPickups;

public class GetFallenStockPickupsQueryHandler : IRequestHandler<GetFallenStockPickupsQuery,
    BaseResponse<GetFallenStockPickupsQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IFallenStockPickupRepository _fallenStockPickupRepository;

    public GetFallenStockPickupsQueryHandler(IFarmRepository farmRepository, ICycleRepository cycleRepository,
        IFallenStockPickupRepository fallenStockPickupRepository)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _fallenStockPickupRepository = fallenStockPickupRepository;
    }

    public async Task<BaseResponse<GetFallenStockPickupsQueryResponse>> Handle(GetFallenStockPickupsQuery request,
        CancellationToken cancellationToken)
    {
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        var cycle = await _cycleRepository.GetAsync(
            new GetCycleByYearIdentifierAndFarmSpec(farm.Id, request.CycleYear, request.CycleIdentifier),
            cancellationToken);

        var fallenStockPickups =
            await _fallenStockPickupRepository.ListAsync<FallenStockPickupsRowDto>(
                new FallenStockPickupsByFarmAndCycleSpec(farm.Id, cycle.Id), cancellationToken);

        return BaseResponse.CreateResponse(new GetFallenStockPickupsQueryResponse
        {
            Items = fallenStockPickups
        });
    }
}

public class FallenStockPickupsProfile : Profile
{
    public FallenStockPickupsProfile()
    {
        CreateMap<FallenStockPickupEntity, FallenStockPickupsRowDto>()
            .ForMember(m => m.FarmName, opt => opt.MapFrom(s => s.Farm.Name))
            .ForMember(dest => dest.CycleText, opt => opt.MapFrom(src => $"{src.Cycle.Identifier}/{src.Cycle.Year}"));
    }
}