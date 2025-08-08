using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Enums;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.FallenStock;

public class
    GetFallenStocksQueryHandler : IRequestHandler<GetFallenStocksQuery, BaseResponse<GetFallenStocksQueryResponse>>
{
    private readonly ICycleRepository _cycleRepository;
    private readonly IFallenStockRepository _fallenStockRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IInsertionRepository _insertionRepository;

    public GetFallenStocksQueryHandler(
        IFallenStockRepository fallenStockRepository,
        IHenhouseRepository henhouseRepository,
        ISaleRepository saleRepository,
        IInsertionRepository insertionRepository, ICycleRepository cycleRepository)
    {
        _fallenStockRepository = fallenStockRepository;
        _henhouseRepository = henhouseRepository;
        _saleRepository = saleRepository;
        _insertionRepository = insertionRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<BaseResponse<GetFallenStocksQueryResponse>> Handle(GetFallenStocksQuery request,
        CancellationToken cancellationToken)
    {
        var viewModel = new GetFallenStocksQueryResponse();

        // KROK 1: Pobierz kurniki, aby zbudować kolumny (bez zmian)
        var henhouses =
            await _henhouseRepository.ListAsync(new HenhousesByFarmIdSpec(request.FarmId), cancellationToken);
        if (henhouses.Count == 0)
        {
            return BaseResponse.CreateResponse(viewModel);
        }

        var cycle = await _cycleRepository.GetAsync(
            new GetCycleByYearIdentifierAndFarmSpec(request.FarmId, request.CycleYear, request.CycleIdentifier),
            cancellationToken);

        viewModel.HenhouseColumns = henhouses
            .Select(h => new ColumnHeaderModel { Id = h.Id.ToString(), Name = h.Name })
            .ToList();

        // KROK 2: Pobierz WSZYSTKIE potrzebne dane na początku
        var insertionData = await _insertionRepository.ListAsync(
            new GetInsertionsByFarmAndCycleSpec(request.FarmId, cycle.Id), cancellationToken);

        var fallenStockData = await _fallenStockRepository.ListAsync(
            new FallenStockByFarmAndCycleSpec(request.FarmId, cycle.Id), cancellationToken);

        var salesData = await _saleRepository.ListAsync(
            new GetSalesByFarmAndCycleSpec(request.FarmId, cycle.Id), cancellationToken);

        // KROK 3: Stwórz PIERWSZY wiersz tabeli - sumę wstawień
        var totalInsertionRow = new TableRowModel
        {
            Id = "summary_insertions",
            RowTitle = "Wstawiono/Data zgłoszenia",
            IsSentToIrz = false
        };
        foreach (var henhouse in henhouses)
        {
            // W cyklu dla kurnika jest tylko jedno wstawienie
            var insertion = insertionData.FirstOrDefault(i => i.HenhouseId == henhouse.Id);
            totalInsertionRow.HenhouseValues[henhouse.Id.ToString()] = insertion?.Quantity;
        }

        viewModel.InsertionRows.Add(totalInsertionRow); // Dodaj jako pierwszy wiersz

        // KROK 4: Stwórz kolejne wiersze dla sztuk padłych (grupowane po dacie)
        var groupedFallenStockByDate = fallenStockData.GroupBy(fs => fs.Date);
        foreach (var dateGroup in groupedFallenStockByDate)
        {
            var firstEntryInGroup = dateGroup.First();
            var fallenStockRow = new TableRowModel
            {
                Id = firstEntryInGroup.InternalGroupId.ToString(),
                RowTitle = dateGroup.Key.ToString("dd.MM.yyyy"),
                IsSentToIrz = firstEntryInGroup.DateIrzSentUtc.HasValue
            };
            foreach (var henhouse in henhouses)
            {
                var entry = dateGroup.FirstOrDefault(fs => fs.HenhouseId == henhouse.Id);
                fallenStockRow.HenhouseValues[henhouse.Id.ToString()] = entry?.Quantity;
            }

            viewModel.InsertionRows.Add(fallenStockRow);
        }

        // KROK 5: Wiersze podsumowania "Ubiórka" i "Sprzedaż" (logika bez zmian)
        var cullingRow = new TableRowModel { Id = "summary_culling", RowTitle = "Ubiórka" };
        foreach (var henhouse in henhouses)
        {
            var cullingSum = salesData
                .Where(s => s.HenhouseId == henhouse.Id && s.Type == SaleType.PartSale)
                .Sum(s => s.Quantity);

            cullingRow.HenhouseValues[henhouse.Id.ToString()] = cullingSum;
        }

        viewModel.SummaryRows.Add(cullingRow);

        var salesRow = new TableRowModel { Id = "summary_sales", RowTitle = "Sprzedaż" };
        foreach (var henhouse in henhouses)
        {
            var salesSum = salesData
                .Where(s => s.HenhouseId == henhouse.Id && s.Type == SaleType.TotalSale)
                .Sum(s => s.Quantity);

            salesRow.HenhouseValues[henhouse.Id.ToString()] = salesSum;
        }

        viewModel.SummaryRows.Add(salesRow);

        // KROK 6: Zaktualizuj obliczanie wiersza "Stan stada"
        var flockStateRow = new TableRowModel { Id = "summary_flock_state", RowTitle = "Stan stada" };
        foreach (var henhouse in henhouses)
        {
            var henhouseIdStr = henhouse.Id.ToString();

            // Pobierz wartość wstawienia z danych o wstawieniach
            var insertionValue = insertionData
                .FirstOrDefault(i => i.HenhouseId == henhouse.Id)?.Quantity ?? 0;

            // Suma upadłych z danych o upadkach
            var fallenStockValue = fallenStockData
                .Where(fs => fs.HenhouseId == henhouse.Id)
                .Sum(fs => fs.Quantity);

            var cullingValue = cullingRow.HenhouseValues.GetValueOrDefault(henhouseIdStr) ?? 0;
            var salesValue = salesRow.HenhouseValues.GetValueOrDefault(henhouseIdStr) ?? 0;

            // Finalna formuła
            flockStateRow.HenhouseValues[henhouseIdStr] =
                insertionValue - fallenStockValue - cullingValue - salesValue;
        }

        flockStateRow.Remaining = flockStateRow.HenhouseValues.Values.Sum(v => v ?? 0);

        viewModel.SummaryRows.Add(flockStateRow);

        viewModel.GrandTotal = flockStateRow.Remaining ?? 0;

        return BaseResponse.CreateResponse(viewModel);
    }
}