namespace FarmsManager.Application.Models.Summary;

public class SummaryFinancialAnalysisRowDto
{
    /// <summary>ID</summary>
    public Guid Id { get; set; }

    /// <summary>Identyfikator (cykl)</summary>
    public string CycleText { get; set; }

    /// <summary>Ferma</summary>
    public string FarmName { get; set; }

    /// <summary>Kurnik</summary>
    public string HenhouseName { get; set; }

    /// <summary>Wylęgarnia</summary>
    public string HatcheryName { get; set; }

    /// <summary>Data wstawienia</summary>
    public DateOnly? InsertionDate { get; set; }

    /// <summary>Data sprzedaży ubiórka</summary>
    public DateOnly? PartSaleDate { get; set; }

    /// <summary>Cena bazowa ubiórka</summary>
    public decimal? PartSaleBasePrice { get; set; }

    /// <summary>Cena ostateczna ubiórka</summary>
    public decimal? PartSaleFinalPrice { get; set; }

    /// <summary>Masa ciała sprzedanych ptaków do rozliczenia ubiórka [kg]</summary>
    public decimal? PartSaleSettlementWeight { get; set; }

    /// <summary>Przychód ubiórka</summary>
    public decimal? PartSaleRevenue { get; set; }

    /// <summary>Data sprzedaży całkowitej</summary>
    public DateOnly? TotalSaleDate { get; set; }

    /// <summary>Cena bazowa sprzedaż całkowita</summary>
    public decimal? TotalSaleBasePrice { get; set; }

    /// <summary>Cena ostateczna sprzedaż całkowita</summary>
    public decimal? TotalSaleFinalPrice { get; set; }

    /// <summary>Masa ciała sprzedanych ptaków do rozliczenia sprzedaż całkowita [kg]</summary>
    public decimal? TotalSaleSettlementWeight { get; set; }

    /// <summary>Przychód sprzedaż całkowita</summary>
    public decimal? TotalSaleRevenue { get; set; }

    /// <summary>Masa ciała sprzedanych ptaków do rozliczenia ubiórka + sprzedaż całkowita [kg]</summary>
    public decimal? CombinedSettlementWeight =>
        PartSaleSettlementWeight.GetValueOrDefault() + TotalSaleSettlementWeight.GetValueOrDefault();

    /// <summary>Przychód ubiórka + sprzedaż całkowita razem</summary>
    public decimal? CombinedRevenue =>
        PartSaleRevenue.GetValueOrDefault() + TotalSaleRevenue.GetValueOrDefault();

    /// <summary>
    /// VAT z przychodów
    /// </summary>
    public decimal? CombinedRevenueVat =>
        PartSaleRevenue.GetValueOrDefault() * 0.08m + TotalSaleRevenue.GetValueOrDefault() * 0.08m;

    /// <summary>Przychód na kg żywca</summary>
    public decimal? RevenuePerKgLiveWeight
    {
        get
        {
            if (CombinedSettlementWeight is null or 0) return null;
            return CombinedRevenue / CombinedSettlementWeight;
        }
    }

    /// <summary>Powierzchnia kurnika [m2]</summary>
    public decimal? HenhouseAreaM2 { get; set; }

    /// <summary>Koszt zużytej paszy</summary>
    public decimal? FeedCost { get; set; }

    /// <summary>Koszt zakupu piskląt</summary>
    public decimal? ChicksCost { get; set; }

    /// <summary>Koszt obsługi weterynaryjnej</summary>
    public decimal? VetCareCost { get; set; }

    /// <summary>Koszt gazu</summary>
    public decimal? GasCost { get; set; }

    /// <summary>Koszty pozostałe</summary>
    public decimal? OtherCosts { get; set; }

    /// <summary>Koszty razem</summary>
    public decimal? TotalCosts =>
        FeedCost.GetValueOrDefault() +
        ChicksCost.GetValueOrDefault() +
        VetCareCost.GetValueOrDefault() +
        GasCost.GetValueOrDefault() +
        OtherCosts.GetValueOrDefault();

    /// <summary>
    /// Vat z wydatków
    /// </summary>
    public decimal? VatCosts => TotalCosts * 0.08m;

    /// <summary>Dochód</summary>
    public decimal? Income => CombinedRevenue - TotalCosts;

    /// <summary>Koszt zużytej paszy na kg żywca</summary>
    public decimal? FeedCostPerKgLiveWeight
    {
        get
        {
            if (CombinedSettlementWeight is null or 0) return null;
            return FeedCost / CombinedSettlementWeight;
        }
    }

    /// <summary>Koszt zakupu piskląt na kg żywca</summary>
    public decimal? ChicksCostPerKgLiveWeight
    {
        get
        {
            if (CombinedSettlementWeight is null or 0) return null;
            return ChicksCost / CombinedSettlementWeight;
        }
    }

    /// <summary>Koszt obsługi weterynaryjnej na kg żywca</summary>
    public decimal? VetCareCostPerKgLiveWeight
    {
        get
        {
            if (CombinedSettlementWeight is null or 0) return null;
            return VetCareCost / CombinedSettlementWeight;
        }
    }

    /// <summary>Koszt gazu na kg żywca</summary>
    public decimal? GasCostPerKgLiveWeight
    {
        get
        {
            if (CombinedSettlementWeight is null or 0) return null;
            return GasCost / CombinedSettlementWeight;
        }
    }

    /// <summary>Koszty pozostałe na kg żywca</summary>
    public decimal? OtherCostsPerKgLiveWeight
    {
        get
        {
            if (CombinedSettlementWeight is null or 0) return null;
            return OtherCosts / CombinedSettlementWeight;
        }
    }

    /// <summary>Koszty razem na kg żywca</summary>
    public decimal? TotalCostsPerKgLiveWeight
    {
        get
        {
            if (CombinedSettlementWeight is null or 0) return null;
            return TotalCosts / CombinedSettlementWeight;
        }
    }

    /// <summary>Dochód na kg żywca</summary>
    public decimal? IncomePerKgLiveWeight => RevenuePerKgLiveWeight - TotalCostsPerKgLiveWeight;

    /// <summary>Koszt zużytej paszy na m2</summary>
    public decimal? FeedCostPerM2
    {
        get
        {
            if (HenhouseAreaM2 is null or 0) return null;
            return FeedCost / HenhouseAreaM2;
        }
    }

    /// <summary>Koszt zakupu piskląt na m2</summary>
    public decimal? ChicksCostPerM2
    {
        get
        {
            if (HenhouseAreaM2 is null or 0) return null;
            return ChicksCost / HenhouseAreaM2;
        }
    }

    /// <summary>Koszt obsługi weterynaryjnej na m2</summary>
    public decimal? VetCareCostPerM2
    {
        get
        {
            if (HenhouseAreaM2 is null or 0) return null;
            return VetCareCost / HenhouseAreaM2;
        }
    }

    /// <summary>Koszt gazu na m2</summary>
    public decimal? GasCostPerM2
    {
        get
        {
            if (HenhouseAreaM2 is null or 0) return null;
            return GasCost / HenhouseAreaM2;
        }
    }

    /// <summary>Koszty pozostałe na m2</summary>
    public decimal? OtherCostsPerM2
    {
        get
        {
            if (HenhouseAreaM2 is null or 0) return null;
            return OtherCosts / HenhouseAreaM2;
        }
    }

    /// <summary>Koszty razem na m2</summary>
    public decimal? TotalCostsPerM2 =>
        FeedCostPerM2.GetValueOrDefault() +
        ChicksCostPerM2.GetValueOrDefault() +
        VetCareCostPerM2.GetValueOrDefault() +
        GasCostPerM2.GetValueOrDefault() +
        OtherCostsPerM2.GetValueOrDefault();

    /// <summary>Przychód na m2</summary>
    public decimal? RevenuePerM2
    {
        get
        {
            if (HenhouseAreaM2 is null or 0) return null;
            return CombinedRevenue / HenhouseAreaM2;
        }
    }

    /// <summary>Dochód na m2</summary>
    public decimal? IncomePerM2 => RevenuePerM2 - TotalCostsPerM2;
}