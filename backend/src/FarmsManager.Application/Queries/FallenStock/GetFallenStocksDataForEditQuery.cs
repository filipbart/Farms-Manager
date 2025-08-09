using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Commands.UtilizationPlants;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.FallenStocks;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.FallenStock;

public record GetFallenStocksDataForEditQueryResponse
{
    public string FarmName { get; init; }
    public string CycleDisplay { get; init; }
    public string UtilizationPlantName { get; init; }
    public DateOnly Date { get; init; }
    public string TypeDesc { get; init; }
    public List<FallenStockEntryDto> Entries { get; init; } = [];

    public record FallenStockEntryDto
    {
        public Guid HenhouseId { get; init; }
        public string HenhouseName { get; init; }
        public int Quantity { get; init; }
    }
}

public record GetFallenStocksDataForEditQuery(Guid InternalGroupId)
    : IRequest<BaseResponse<GetFallenStocksDataForEditQueryResponse>>;

public class GetFallenStocksDataForEditQueryHandler : IRequestHandler<GetFallenStocksDataForEditQuery,
    BaseResponse<GetFallenStocksDataForEditQueryResponse>>
{
    private readonly IFallenStockRepository _fallenStockRepository;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IUtilizationPlantRepository _utilizationPlantRepository;

    public GetFallenStocksDataForEditQueryHandler(
        IFallenStockRepository fallenStockRepository,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IHenhouseRepository henhouseRepository,
        IUtilizationPlantRepository utilizationPlantRepository)
    {
        _fallenStockRepository = fallenStockRepository;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _utilizationPlantRepository = utilizationPlantRepository;
    }

    public async Task<BaseResponse<GetFallenStocksDataForEditQueryResponse>> Handle(
        GetFallenStocksDataForEditQuery request, CancellationToken ct)
    {
        var fallenStockEntries = await _fallenStockRepository.ListAsync(
            new GetFallenStockByInternalGroupIdSpec(request.InternalGroupId), ct);

        if (fallenStockEntries.Count == 0)
        {
            throw new Exception("Nie znaleziono zgłoszeń sztuk padłych o podanym identyfikatorze grupy.");
        }

        var firstEntry = fallenStockEntries.First();
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(firstEntry.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(firstEntry.CycleId), ct);

        UtilizationPlantEntity utilizationPlant = null;
        if (firstEntry.UtilizationPlantId.HasValue)
        {
            utilizationPlant =
                await _utilizationPlantRepository.GetAsync(
                    new UtilizationPlantByIdSpec(firstEntry.UtilizationPlantId.Value), ct);
        }

        var entryDtos = new List<GetFallenStocksDataForEditQueryResponse.FallenStockEntryDto>();
        foreach (var entry in fallenStockEntries)
        {
            var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(entry.HenhouseId), ct);
            entryDtos.Add(new GetFallenStocksDataForEditQueryResponse.FallenStockEntryDto
            {
                HenhouseId = entry.HenhouseId,
                HenhouseName = henhouse.Name,
                Quantity = entry.Quantity
            });
        }

        var responseData = new GetFallenStocksDataForEditQueryResponse
        {
            FarmName = farm.Name,
            CycleDisplay = $"{cycle.Identifier}/{cycle.Year}",
            UtilizationPlantName = utilizationPlant?.Name,
            Date = firstEntry.Date,
            TypeDesc = firstEntry.Type.GetDescription(),
            Entries = entryDtos.OrderBy(e => e.HenhouseName)
                .ToList()
        };

        return BaseResponse.CreateResponse(responseData);
    }
}