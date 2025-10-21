using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Summary;
using FarmsManager.Application.Queries.Insertions;
using FarmsManager.Application.Queries.Sales;
using FarmsManager.Application.Queries.Summary.ProductionAnalysis;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Enums;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using MediatR;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;

namespace FarmsManager.Application.Queries.Summary.FinancialAnalysis;

// Pomocniczy rekord do przechowywania zagregowanych wyników sprzedaży
internal record SaleMetrics(
    decimal? BasePrice,
    decimal? FinalPrice,
    decimal? SettlementWeight,
    decimal? Revenue,
    DateOnly? SaleDate
);

public class SummaryFinancialAnalysisQueryHandler : IRequestHandler<SummaryFinancialAnalysisQuery,
    BaseResponse<SummaryFinancialAnalysisQueryResponse>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IInsertionRepository _insertionRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IGasConsumptionRepository _gasConsumptionRepository;
    private readonly IEmployeePayslipRepository _employeePayslipRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IProductionDataRemainingFeedRepository _productionDataRemainingFeedRepository;
    private readonly IProductionDataTransferFeedRepository _productionDataTransferFeedRepository;

    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public SummaryFinancialAnalysisQueryHandler(ISaleRepository saleRepository,
        IInsertionRepository insertionRepository, IFeedInvoiceRepository feedInvoiceRepository,
        IGasConsumptionRepository gasConsumptionRepository,
        IEmployeePayslipRepository employeePayslipRepository,
        IExpenseProductionRepository expenseProductionRepository,
        IProductionDataRemainingFeedRepository productionDataRemainingFeedRepository,
        IProductionDataTransferFeedRepository productionDataTransferFeedRepository, IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _saleRepository = saleRepository;
        _insertionRepository = insertionRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _gasConsumptionRepository = gasConsumptionRepository;
        _employeePayslipRepository = employeePayslipRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _productionDataRemainingFeedRepository = productionDataRemainingFeedRepository;
        _productionDataTransferFeedRepository = productionDataTransferFeedRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<SummaryFinancialAnalysisQueryResponse>> Handle(
        SummaryFinancialAnalysisQuery request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var allInsertions =
            await _insertionRepository.ListAsync(new GetAllInsertionsSpec(request.Filters, true, accessibleFarmIds, user.IsAdmin),
                ct);
        if (allInsertions.Count == 0)
        {
            return BaseResponse.CreateResponse(new SummaryFinancialAnalysisQueryResponse());
        }

        var henhouseIds = allInsertions.Select(i => i.HenhouseId).Distinct().ToList();
        var farmIds = allInsertions.Select(i => i.FarmId).Distinct().ToList();

        var allSales =
            await _saleRepository.ListAsync(
                new GetAllSalesSpec(new GetSalesQueryFilters { HenhouseIds = henhouseIds }, false, accessibleFarmIds, user.IsAdmin),
                ct);
        var allExpenses = await _expenseProductionRepository.ListAsync(new ExpensesProductionsByFarmsSpec(farmIds), ct);
        var gasConsumptions = await _gasConsumptionRepository.ListAsync(new GasConsumptionsByFarmsSpec(farmIds), ct);
        var employeesPayslips =
            await _employeePayslipRepository.ListAsync(new EmployeePayslipsByFarmsSpec(farmIds), ct);
        var feedInvoices = await _feedInvoiceRepository.ListAsync(new FeedsDeliveriesByHenhousesSpec(henhouseIds), ct);
        var feedRemainings =
            await _productionDataRemainingFeedRepository.ListAsync(
                new ProductionDataRemainingFeedByHenhousesSpec(henhouseIds), ct);
        var feedTransfers =
            await _productionDataTransferFeedRepository.ListAsync(
                new ProductionDataTransferFeedByHenhousesSpec(henhouseIds), ct);

        var salesLookup = allSales.ToLookup(s => (s.FarmId, s.HenhouseId, s.CycleId));
        var gasLookup = gasConsumptions.ToLookup(g => (g.FarmId, g.CycleId));
        var employeesPayslipsLookup = employeesPayslips.ToLookup(e => (e.FarmId, e.CycleId));
        var expensesLookup = allExpenses.ToLookup(e => (e.FarmId, e.CycleId));
        var feedInvoiceLookup = feedInvoices.ToLookup(fi => (fi.FarmId, fi.HenhouseId, fi.CycleId));
        var feedRemainingLookup = feedRemainings.ToLookup(fr => (fr.FarmId, fr.HenhouseId, fr.CycleId));
        var feedTransfersFromLookup = feedTransfers.ToLookup(ft => (ft.FromFarmId, ft.FromHenhouseId, ft.FromCycleId));
        var feedTransfersToLookup = feedTransfers.ToLookup(ft => (ft.ToFarmId, ft.ToHenhouseId, ft.ToCycleId));
        var feedRemainingByHenhouseCycleLookup =
            feedRemainings.ToLookup(fr => (fr.FarmId, fr.HenhouseId, fr.Cycle.Year, fr.Cycle.Identifier));

        var resultList = new List<SummaryFinancialAnalysisRowDto>();

        foreach (var insertion in allInsertions)
        {
            var insertionKey = (insertion.FarmId, insertion.HenhouseId, insertion.CycleId);
            var cycleSales = salesLookup[insertionKey].ToList();

            var partSales = cycleSales.Where(s => s.Type == SaleType.PartSale).ToList();
            var totalSales = cycleSales.Where(s => s.Type == SaleType.TotalSale).ToList();

            var partSaleMetrics = AggregateSalesMetrics(partSales);
            var totalSaleMetrics = AggregateSalesMetrics(totalSales);

            var feedConsumedCost = CalculateFeedCost(insertion, feedInvoiceLookup[insertionKey],
                feedRemainingLookup[insertionKey], feedRemainingByHenhouseCycleLookup,
                feedTransfersFromLookup[insertionKey], feedTransfersToLookup[insertionKey]);

            var expenseKey = (insertion.FarmId, insertion.CycleId);
            var cycleExpenses = expensesLookup[expenseKey]?.ToList();

            var chicksCost = CalculateExpenses(insertion, allInsertions, cycleExpenses, "Zakup piskląt");
            var vetCareCost = CalculateExpenses(insertion, allInsertions, cycleExpenses, "Usługa weterynaryjna");
            var otherCosts = CalculateOtherExpenses(insertion, allInsertions, cycleExpenses, [
                "Zakup piskląt", "Usługa weterynaryjna"
            ]);
            var gasCost = CalculateGasCost(insertion, gasLookup[expenseKey], allInsertions);
            var employeePayslipsCost = CalculateEmployeePayslipsCost(insertion, allInsertions,
                employeesPayslipsLookup[expenseKey]);
            otherCosts += employeePayslipsCost;

            var row = new SummaryFinancialAnalysisRowDto
            {
                Id = Guid.NewGuid(),
                CycleText = $"{insertion.Cycle.Identifier}/{insertion.Cycle.Year}",
                FarmName = insertion.Farm.Name,
                HenhouseName = insertion.Henhouse.Name,
                HatcheryName = insertion.Hatchery.Name,
                InsertionDate = insertion.InsertionDate,

                PartSaleDate = partSaleMetrics.SaleDate,
                PartSaleBasePrice = partSaleMetrics.BasePrice,
                PartSaleFinalPrice = partSaleMetrics.FinalPrice,
                PartSaleSettlementWeight = partSaleMetrics.SettlementWeight,
                PartSaleRevenue = partSaleMetrics.Revenue,

                TotalSaleDate = totalSaleMetrics.SaleDate,
                TotalSaleBasePrice = totalSaleMetrics.BasePrice,
                TotalSaleFinalPrice = totalSaleMetrics.FinalPrice,
                TotalSaleSettlementWeight = totalSaleMetrics.SettlementWeight,
                TotalSaleRevenue = totalSaleMetrics.Revenue,

                HenhouseAreaM2 = insertion.Henhouse.Area,
                FeedCost = feedConsumedCost,
                ChicksCost = chicksCost,
                VetCareCost = vetCareCost,
                GasCost = gasCost,
                OtherCosts = otherCosts,
            };

            resultList.Add(row);
        }

        return BaseResponse.CreateResponse(new SummaryFinancialAnalysisQueryResponse
        {
            TotalRows = resultList.Count,
            Items = resultList
        });
    }

    private static SaleMetrics AggregateSalesMetrics(IReadOnlyCollection<SaleEntity> sales)
    {
        if (sales == null || sales.Count == 0)
        {
            return new SaleMetrics(null, null, null, null, null);
        }

        decimal kgForSettlementSum = 0;
        decimal kgForSettlementBasePriceSum = 0;
        decimal kgForSettlementFinalPriceSum = 0;
        decimal totalRevenue = 0;

        foreach (var saleEntity in sales)
        {
            var kgForSettlement = saleEntity.Weight - saleEntity.DeadWeight - saleEntity.ConfiscatedWeight;
            kgForSettlementSum += kgForSettlement;
            kgForSettlementBasePriceSum += kgForSettlement * saleEntity.BasePrice;
            kgForSettlementFinalPriceSum += kgForSettlement * saleEntity.PriceWithExtras;
            totalRevenue += kgForSettlement * saleEntity.PriceWithExtras;
        }

        if (kgForSettlementSum == 0)
        {
            return new SaleMetrics(null, null, 0, 0, sales.Min(s => s.SaleDate));
        }

        return new SaleMetrics(
            BasePrice: kgForSettlementBasePriceSum / kgForSettlementSum,
            FinalPrice: kgForSettlementFinalPriceSum / kgForSettlementSum,
            SettlementWeight: kgForSettlementSum,
            Revenue: totalRevenue,
            SaleDate: sales.Min(s => s.SaleDate)
        );
    }

    private static decimal? AllocateProportionalCost(InsertionEntity insertion,
        IEnumerable<InsertionEntity> allInsertions,
        decimal totalCostForFarm)
    {
        var farmHenhousesArea = allInsertions
            .Where(i => i.FarmId == insertion.FarmId && i.CycleId == insertion.CycleId)
            .Sum(i => i.Henhouse.Area);

        if (farmHenhousesArea == 0) return null;

        return totalCostForFarm / farmHenhousesArea * insertion.Henhouse.Area;
    }

    private static decimal? CalculateGasCost(InsertionEntity insertion,
        IEnumerable<GasConsumptionEntity> gasConsumptions, IEnumerable<InsertionEntity> allInsertions)
    {
        var farmHenhousesArea = allInsertions
            .Where(i => i.FarmId == insertion.FarmId && i.CycleId == insertion.CycleId)
            .Sum(i => i.Henhouse.Area);

        if (farmHenhousesArea == 0) return null;

        var totalCost = gasConsumptions.Sum(g => g.Cost);

        return totalCost / farmHenhousesArea * insertion.Henhouse.Area;
    }

    private static decimal? CalculateEmployeePayslipsCost(InsertionEntity insertion,
        IEnumerable<InsertionEntity> allInsertions,
        IEnumerable<EmployeePayslipEntity> payslips)
    {
        var totalCost = payslips.Sum(p =>
            p.BankTransferAmount + p.BonusAmount + p.OvertimePay - p.Deductions + p.OtherAllowances);
        return AllocateProportionalCost(insertion, allInsertions, totalCost);
    }

    private static decimal? CalculateExpenses(InsertionEntity insertion, IEnumerable<InsertionEntity> allInsertions,
        IEnumerable<ExpenseProductionEntity> expenses, string expenseType)
    {
        var totalCost = expenses
            .Where(e => string.Equals(e.ExpenseContractor.ExpenseType.Name, expenseType,
                StringComparison.OrdinalIgnoreCase))
            .Sum(e => e.SubTotal);
        return AllocateProportionalCost(insertion, allInsertions, totalCost);
    }

    private static decimal? CalculateOtherExpenses(InsertionEntity insertion,
        IEnumerable<InsertionEntity> allInsertions,
        IEnumerable<ExpenseProductionEntity> expenses, string[] excludeExpenseTypes)
    {
        var totalCost = expenses
            .Where(e => !excludeExpenseTypes.Contains(e.ExpenseContractor.ExpenseType.Name,
                StringComparer.OrdinalIgnoreCase))
            .Sum(e => e.SubTotal);
        return AllocateProportionalCost(insertion, allInsertions, totalCost);
    }

    private static decimal CalculateFeedCost(InsertionEntity insertion,
        IEnumerable<FeedInvoiceEntity> invoices,
        IEnumerable<ProductionDataRemainingFeedEntity> remainings,
        ILookup<(Guid, Guid, int, int), ProductionDataRemainingFeedEntity> remainingByCycleLookup,
        IEnumerable<ProductionDataTransferFeedEntity> transfersFrom,
        IEnumerable<ProductionDataTransferFeedEntity> transfersTo)
    {
        var previousCycleIdentifier = insertion.Cycle.Identifier == 1 ? 6 : insertion.Cycle.Identifier - 1;
        var previousCycleYear = insertion.Cycle.Identifier == 1 ? insertion.Cycle.Year - 1 : insertion.Cycle.Year;
        var previousCycleKey = (insertion.FarmId, insertion.HenhouseId, previousCycleYear, previousCycleIdentifier);

        var sumFeedInvoices = invoices.Sum(fi => fi.SubTotal);
        var sumFeedRemainings = remainings.Sum(t => t.RemainingValue);
        var sumFeedRemainingsPreviousCycle = remainingByCycleLookup[previousCycleKey].Sum(t => t.RemainingValue);
        var sumFeedTransfersFrom = transfersFrom.Sum(t => t.Value);
        var sumFeedTransfersTo = transfersTo.Sum(t => t.Value);

        return sumFeedInvoices - sumFeedRemainings + sumFeedRemainingsPreviousCycle - sumFeedTransfersFrom +
               sumFeedTransfersTo;
    }
}