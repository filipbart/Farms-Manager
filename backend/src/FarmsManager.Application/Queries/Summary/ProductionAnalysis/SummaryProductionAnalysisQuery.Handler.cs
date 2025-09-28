using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.ProductionData;
using FarmsManager.Application.Models.Summary;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Queries.Insertions;
using FarmsManager.Application.Queries.ProductionData.Failures;
using FarmsManager.Application.Queries.ProductionData.Weighings;
using FarmsManager.Application.Queries.Sales;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Enums;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using MediatR;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;

namespace FarmsManager.Application.Queries.Summary.ProductionAnalysis;

internal record SaleAggregate(
    DateOnly? SaleDate,
    int? SoldCount,
    decimal? SoldWeight,
    decimal? FarmerWeight,
    int? AgeInDays,
    int? DeadCount,
    decimal? DeadWeight,
    int? ConfiscatedCount,
    decimal? ConfiscatedWeight
);

public class SummaryProductionAnalysisQueryHandler : IRequestHandler<SummaryProductionAnalysisQuery,
    BaseResponse<SummaryProductionAnalysisQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IInsertionRepository _insertionRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IGasConsumptionRepository _gasConsumptionRepository;
    private readonly IProductionDataFailureRepository _productionDataFailureRepository;
    private readonly IProductionDataWeightStandardRepository _productionDataWeightStandardRepository;
    private readonly IProductionDataRemainingFeedRepository _productionDataRemainingFeedRepository;
    private readonly IProductionDataTransferFeedRepository _productionDataTransferFeedRepository;

    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public SummaryProductionAnalysisQueryHandler(IFarmRepository farmRepository, ISaleRepository saleRepository,
        IInsertionRepository insertionRepository, IFeedInvoiceRepository feedInvoiceRepository,
        IGasConsumptionRepository gasConsumptionRepository,
        IProductionDataFailureRepository productionDataFailureRepository,
        IProductionDataWeightStandardRepository productionDataWeightStandardRepository,
        IProductionDataRemainingFeedRepository productionDataRemainingFeedRepository,
        IProductionDataTransferFeedRepository productionDataTransferFeedRepository, IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _farmRepository = farmRepository;
        _saleRepository = saleRepository;
        _insertionRepository = insertionRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _gasConsumptionRepository = gasConsumptionRepository;
        _productionDataFailureRepository = productionDataFailureRepository;
        _productionDataWeightStandardRepository = productionDataWeightStandardRepository;
        _productionDataRemainingFeedRepository = productionDataRemainingFeedRepository;
        _productionDataTransferFeedRepository = productionDataTransferFeedRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<SummaryProductionAnalysisQueryResponse>> Handle(
        SummaryProductionAnalysisQuery request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var allInsertions =
            await _insertionRepository.ListAsync(new GetAllInsertionsSpec(request.Filters, true, accessibleFarmIds),
                ct);
        if (allInsertions.Count == 0)
        {
            return BaseResponse.CreateResponse(new SummaryProductionAnalysisQueryResponse());
        }

        var henhouseIds = allInsertions.Select(i => i.HenhouseId).Distinct().ToList();
        var farmIds = allInsertions.Select(i => i.FarmId).Distinct().ToList();

        var farms = (await _farmRepository.ListAsync(new GetAllFarmsSpec(null), ct)).ToDictionary(f => f.Id);
        var allWeightStandards =
            (await _productionDataWeightStandardRepository.ListAsync(new GetAllWeightStandardsSpec(), ct))
            .ToDictionary(ws => ws.Day);

        var allSales =
            await _saleRepository.ListAsync(
                new GetAllSalesSpec(new GetSalesQueryFilters { HenhouseIds = henhouseIds }, false, accessibleFarmIds),
                ct);
        var allFailures = await _productionDataFailureRepository.ListAsync(
            new GetAllProductionDataFailuresSpec(new ProductionDataQueryFilters { HenhouseIds = henhouseIds }, false,
                accessibleFarmIds),
            ct);
        var gasConsumptions = await _gasConsumptionRepository.ListAsync(new GasConsumptionsByFarmsSpec(farmIds), ct);
        var feedInvoices = await _feedInvoiceRepository.ListAsync(new FeedsDeliveriesByHenhousesSpec(henhouseIds), ct);
        var feedRemainings =
            await _productionDataRemainingFeedRepository.ListAsync(
                new ProductionDataRemainingFeedByHenhousesSpec(henhouseIds), ct);
        var feedTransfers =
            await _productionDataTransferFeedRepository.ListAsync(
                new ProductionDataTransferFeedByHenhousesSpec(henhouseIds), ct);

        var salesLookup = allSales.ToLookup(s => (s.FarmId, s.HenhouseId, s.CycleId));
        var failuresLookup = allFailures.ToLookup(f => (f.FarmId, f.HenhouseId, f.CycleId));
        var gasLookup = gasConsumptions.ToLookup(g => (g.FarmId, g.CycleId));
        var feedInvoiceLookup = feedInvoices.ToLookup(fi => (fi.FarmId, fi.HenhouseId, fi.CycleId));
        var feedRemainingLookup = feedRemainings.ToLookup(fr => (fr.FarmId, fr.HenhouseId, fr.CycleId));

        var feedTransfersFromLookup = feedTransfers.ToLookup(ft => (ft.FromFarmId, ft.FromHenhouseId, ft.FromCycleId));
        var feedTransfersToLookup = feedTransfers.ToLookup(ft => (ft.ToFarmId, ft.ToHenhouseId, ft.ToCycleId));

        var feedRemainingByHenhouseCycleLookup =
            feedRemainings.ToLookup(fr => (fr.FarmId, fr.HenhouseId, fr.Cycle.Year, fr.Cycle.Identifier));

        var resultList = new List<SummaryProductionAnalysisRowDto>();
        var id = 1;


        foreach (var insertion in allInsertions)
        {
            var insertionKey = (insertion.FarmId, insertion.HenhouseId, insertion.CycleId);
            var cycleSales = salesLookup[insertionKey].ToList();

            var partSalesAgg = AggregateSales(cycleSales, SaleType.PartSale, insertion.InsertionDate);
            var totalSalesAgg = AggregateSales(cycleSales, SaleType.TotalSale, insertion.InsertionDate);

            var cycleFailures = failuresLookup[insertionKey];
            var productionDataFailureEntities = cycleFailures?.ToList();
            var deadCountCycle = productionDataFailureEntities.Sum(f => f.DeadCount);
            var defectiveCountCycle = productionDataFailureEntities.Sum(f => f.DefectiveCount);

            var feedConsumed = CalculateFeedConsumption(insertion, feedInvoiceLookup[insertionKey],
                feedRemainingLookup[insertionKey], feedRemainingByHenhouseCycleLookup,
                feedTransfersFromLookup[insertionKey], feedTransfersToLookup[insertionKey]);

            var gasConsumed = CalculateGasConsumption(insertion, gasLookup, allInsertions);


            _ = allWeightStandards.TryGetValue(partSalesAgg.AgeInDays ?? 0, out var partSaleWeightStandard);
            _ = allWeightStandards.TryGetValue(totalSalesAgg.AgeInDays ?? 0, out var totalSaleWeightStandard);

            var row = new SummaryProductionAnalysisRowDto
            {
                Id = id,
                CycleText = $"{insertion.Cycle.Identifier}/{insertion.Cycle.Year}",
                FarmName = insertion.Farm.Name,
                HenhouseName = insertion.Henhouse.Name,
                HatcheryName = insertion.Hatchery.Name,
                InsertionDate = insertion.InsertionDate,
                InsertionQuantity = insertion.Quantity,

                PartSaleDate = partSalesAgg.SaleDate,
                PartSaleSoldCount = partSalesAgg.SoldCount,
                PartSaleSoldWeight = partSalesAgg.SoldWeight,
                PartSaleFarmerWeight = partSalesAgg.FarmerWeight,
                PartSaleAgeInDays = partSalesAgg.AgeInDays,
                PartSaleDeadCount = partSalesAgg.DeadCount,
                PartSaleDeadWeight = partSalesAgg.DeadWeight,
                PartSaleConfiscatedCount = partSalesAgg.ConfiscatedCount,
                PartSaleConfiscatedWeight = partSalesAgg.ConfiscatedWeight,
                PartSaleAvgWeightDeviation = partSalesAgg.SoldWeight / partSalesAgg.SoldCount * 1000 -
                                             partSaleWeightStandard?.Weight,

                TotalSaleDate = totalSalesAgg.SaleDate,
                TotalSaleSoldCount = totalSalesAgg.SoldCount,
                TotalSaleSoldWeight = totalSalesAgg.SoldWeight,
                TotalSaleFarmerWeight = totalSalesAgg.FarmerWeight,
                TotalSaleAgeInDays = totalSalesAgg.AgeInDays,
                TotalSaleDeadCount = totalSalesAgg.DeadCount,
                TotalSaleDeadWeight = totalSalesAgg.DeadWeight,
                TotalSaleConfiscatedCount = totalSalesAgg.ConfiscatedCount,
                TotalSaleConfiscatedWeight = totalSalesAgg.ConfiscatedWeight,
                TotalSaleAvgWeightDeviation = totalSalesAgg.SoldWeight / totalSalesAgg.SoldCount * 1000 -
                                              totalSaleWeightStandard?.Weight,

                CombinedSoldCount = partSalesAgg.SoldCount.GetValueOrDefault() +
                                    totalSalesAgg.SoldCount.GetValueOrDefault(),
                CombinedSoldWeight = partSalesAgg.SoldWeight.GetValueOrDefault() +
                                     totalSalesAgg.SoldWeight.GetValueOrDefault(),
                CombinedFarmerWeight = partSalesAgg.FarmerWeight.GetValueOrDefault() +
                                       totalSalesAgg.FarmerWeight.GetValueOrDefault(),

                DeadCountCycle = deadCountCycle,
                DefectiveCountCycle = defectiveCountCycle,
                FeedConsumedTons = feedConsumed,
                HouseAreaM2 = insertion.Henhouse.Area,
                GasConsumptionLiters = gasConsumed
            };

            resultList.Add(row);
            id++;
        }

        return BaseResponse.CreateResponse(new SummaryProductionAnalysisQueryResponse
        {
            TotalRows = resultList.Count,
            Items = resultList
        });
    }


    private static SaleAggregate AggregateSales(List<SaleEntity> sales, SaleType type, DateOnly insertionDate)
    {
        var relevantSales = sales.Where(s => s.Type == type).ToList();
        if (relevantSales.Count == 0)
        {
            return new SaleAggregate(null, null, null, null, null, null, null, null, null);
        }

        var minSaleDate = relevantSales.Min(s => s.SaleDate);
        var ageInDays = minSaleDate.DayNumber - insertionDate.DayNumber - 1;

        return new SaleAggregate(
            SaleDate: minSaleDate,
            SoldCount: relevantSales.Sum(s => s.Quantity),
            SoldWeight: relevantSales.Sum(s => s.Weight),
            FarmerWeight: relevantSales.Sum(s => s.FarmerWeight),
            AgeInDays: ageInDays,
            DeadCount: relevantSales.Sum(s => s.DeadCount),
            DeadWeight: relevantSales.Sum(s => s.DeadWeight),
            ConfiscatedCount: relevantSales.Sum(s => s.ConfiscatedCount),
            ConfiscatedWeight: relevantSales.Sum(s => s.ConfiscatedWeight)
        );
    }


    private static decimal CalculateFeedConsumption(InsertionEntity insertion,
        IEnumerable<FeedInvoiceEntity> invoices,
        IEnumerable<ProductionDataRemainingFeedEntity> remainings,
        ILookup<(Guid, Guid, int, int), ProductionDataRemainingFeedEntity> remainingByCycleLookup,
        IEnumerable<ProductionDataTransferFeedEntity> transfersFrom,
        IEnumerable<ProductionDataTransferFeedEntity> transfersTo)
    {
        var previousCycleIdentifier = insertion.Cycle.Identifier == 1 ? 6 : insertion.Cycle.Identifier - 1;
        var previousCycleYear = insertion.Cycle.Identifier == 1 ? insertion.Cycle.Year - 1 : insertion.Cycle.Year;
        var previousCycleKey = (insertion.FarmId, insertion.HenhouseId, previousCycleYear, previousCycleIdentifier);

        var sumFeedInvoices = invoices.Sum(fi => fi.Quantity);
        var sumFeedRemainings = remainings.Sum(t => t.RemainingTonnage);
        var sumFeedRemainingsPreviousCycle = remainingByCycleLookup[previousCycleKey].Sum(t => t.RemainingTonnage);
        var sumFeedTransfersFrom = transfersFrom.Sum(t => t.Tonnage);
        var sumFeedTransfersTo = transfersTo.Sum(t => t.Tonnage);

        return sumFeedInvoices - sumFeedRemainings + sumFeedRemainingsPreviousCycle - sumFeedTransfersFrom +
               sumFeedTransfersTo;
    }


    private static decimal? CalculateGasConsumption(InsertionEntity insertion,
        ILookup<(Guid, Guid), GasConsumptionEntity> gasLookup, IEnumerable<InsertionEntity> allInsertions)
    {
        var farmHenhousesArea = allInsertions
            .Where(i => i.FarmId == insertion.FarmId && i.CycleId == insertion.CycleId)
            .Sum(i => i.Henhouse.Area);

        if (farmHenhousesArea == 0) return null;

        var gasKey = (insertion.FarmId, insertion.CycleId);
        var consumedGasForFarm = gasLookup[gasKey].Sum(g => g.QuantityConsumed);

        return consumedGasForFarm / farmHenhousesArea * insertion.Henhouse.Area;
    }
}