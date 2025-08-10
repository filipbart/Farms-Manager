namespace FarmsManager.Application.Models.Summary;

public class SummaryProductionAnalysisRowDto
{
    /// <summary>Identyfikator (cykl)</summary>
    public string CycleText { get; set; }

    /// <summary>Ferma</summary>
    public string FarmName { get; set; }

    /// <summary>Kurnik</summary>
    public string HenhouseName { get; set; }

    /// <summary>Wylęgarnia</summary>
    public string HatcheryName { get; set; }

    /// <summary>Data wstawienia</summary>
    public DateOnly InsertionDate { get; set; }

    /// <summary>Sztuki wstawione</summary>
    public int? InsertionQuantity { get; set; }

    /// <summary>Data sprzedaży ubiórka</summary>
    public DateOnly? PartSaleDate { get; set; }

    /// <summary>Sztuki sprzedane ubiórka</summary>
    public int? PartSaleSoldCount { get; set; }

    /// <summary>Masa sprzedanych ptaków ubojnia ubiórka [kg]</summary>
    public decimal? PartSaleSoldWeight { get; set; }

    /// <summary>Śr. Masa ciała sprzedanych ptaków ubiórka [kg]</summary>
    public decimal? PartSaleAvgWeight { get; set; }

    /// <summary>Waga hodowcy ubiórka [kg]</summary>
    public decimal? PartSaleFarmerWeight { get; set; }

    /// <summary>Doba ubiórki</summary>
    public int? PartSaleAgeInDays { get; set; }

    /// <summary>Różnice wagowe ubiórka [%]</summary>
    public decimal? PartSaleWeightDiffPct { get; set; }

    /// <summary>Kurczęta martwe ubiórka [szt]</summary>
    public int? PartSaleDeadCount { get; set; }

    /// <summary>Kurczęta martwe ubiórka [kg]</summary>
    public decimal? PartSaleDeadWeight { get; set; }

    /// <summary>Konfiskata ubiórka [szt]</summary>
    public int? PartSaleConfiscatedCount { get; set; }

    /// <summary>Konfiskata ubiórka [kg]</summary>
    public decimal? PartSaleConfiscatedWeight { get; set; }

    /// <summary>Liczba sztuk uwzględnionych do rozliczenia ubiórka</summary>
    public int? PartSaleSettlementCount { get; set; }

    /// <summary>Masa ciała sprzedanych ptaków do rozliczenia ubióka [kg]</summary>
    public decimal? PartSaleSettlementWeight { get; set; }

    /// <summary>Data sprzedaży całkowitej</summary>
    public DateOnly? TotalSaleDate { get; set; }

    /// <summary>Sztuki sprzedane sprzedaż całkowita</summary>
    public int? TotalSoldCount { get; set; }

    /// <summary>Masa sprzedanych ptaków Ubojnia sprzedaż całkowita</summary>
    public decimal? TotalSoldWeight { get; set; }

    /// <summary>Śr. Masa ciała sprzedanych ptaków sprzedaż całkowita</summary>
    public decimal? TotalAvgWeight { get; set; }

    /// <summary>Waga hodowcy sprzedaż całkowita [kg]</summary>
    public decimal? TotalFarmerWeight { get; set; }

    /// <summary>Doba sprzedaż całkowita</summary>
    public int? TotalAgeInDays { get; set; }

    /// <summary>Różnice wagowe sprzedaż całkowita [%]</summary>
    public decimal? TotalWeightDiffPct { get; set; }

    /// <summary>Kurczęta martwe sprzedaż całkowita [szt]</summary>
    public int? TotalDeadCount { get; set; }

    /// <summary>Kurczęta martwe sprzedaż całkowita [kg]</summary>
    public decimal? TotalDeadWeight { get; set; }

    /// <summary>Konfiskata sprzedaż całkowita [szt]</summary>
    public int? TotalConfiscatedCount { get; set; }

    /// <summary>Konfiskata sprzedaż całkowita [kg]</summary>
    public decimal? TotalConfiscatedWeight { get; set; }

    /// <summary>Liczba sztuk uwzględnionych do rozliczenia sprzedaż całkowita</summary>
    public int? TotalSettlementCount { get; set; }

    /// <summary>Masa ciała sprzedanych ptaków do rozliczenia sprzedaż całkowita</summary>
    public decimal? TotalSettlementWeight { get; set; }

    /// <summary>Sztuki sprzedane razem</summary>
    public int? CombinedSoldCount { get; set; }

    /// <summary>Masa sprzedanych ptaków Ubojnia razem [kg]</summary>
    public decimal? CombinedSoldWeight { get; set; }

    /// <summary>Śr. Masa ciała sprzedanych ptaków razem [kg]</summary>
    public decimal? CombinedAvgWeight { get; set; }

    /// <summary>Waga hodowcy razem [kg]</summary>
    public decimal? CombinedFarmerWeight { get; set; }

    /// <summary>Liczba sztuk uwzględnionych do rozliczenia ubiórka + sprzedaż całkowita</summary>
    public int? CombinedSettlementCount { get; set; }

    /// <summary>Masa ciała sprzedanych ptaków do rozliczenia ubióka + sprzedaż całkowita [kg]</summary>
    public decimal? CombinedSettlementWeight { get; set; }

    /// <summary>Średnia doba życia ubiórka + sprzedaż całkowita</summary>
    public decimal? CombinedAvgAgeInDays { get; set; }

    /// <summary>Przeżywalność z konfiskatami i upadkami na ubojni [%]</summary>
    public decimal? SurvivalRatePct { get; set; }

    /// <summary>Sztuki padłe z cyklu produkcyjnego</summary>
    public int? DeadCountCycle { get; set; }

    /// <summary>Sztuki wybrakowane z cyklu produkcyjnego</summary>
    public int? RejectedCountCycle { get; set; }

    /// <summary>% sztuk padłych z cyklu produkcyjnego</summary>
    public decimal? DeadPctCycle { get; set; }

    /// <summary>% sztuk wybrakowanych z cyklu produkcyjnego</summary>
    public decimal? RejectedPctCycle { get; set; }

    /// <summary>% sztuk padłych i wybrakowanych z cyklu produkcyjnego</summary>
    public decimal? DeadAndRejectedPctCycle { get; set; }

    /// <summary>Spożyta pasza [t]</summary>
    public decimal? FeedConsumedTons { get; set; }

    /// <summary>FCR (z padłymi i konfiskatami)</summary>
    public decimal? FcrWithLosses { get; set; }

    /// <summary>FCR (bez padłych i konfiskat)</summary>
    public decimal? FcrWithoutLosses { get; set; }

    /// <summary>Punkty</summary>
    public decimal? Points { get; set; }

    /// <summary>EWW</summary>
    public decimal? Eww { get; set; }

    /// <summary>Powierzchnia kurnika [m2]</summary>
    public decimal? HouseAreaM2 { get; set; }

    /// <summary>Liczba kg uzyskanych z m2 (przed konfiskatami)</summary>
    public decimal? KgPerM2BeforeConf { get; set; }

    /// <summary>Liczba kg uzyskanych z m2 (po konfiskatach)</summary>
    public decimal? KgPerM2AfterConf { get; set; }

    /// <summary>Zużycie gazu [L]</summary>
    public decimal? GasConsumptionLiters { get; set; }

    /// <summary>Zużycie gazu na 1m2 powierzchni</summary>
    public decimal? GasConsumptionPerM2 { get; set; }

    /// <summary>Bilans sztuk na koniec cyklu</summary>
    public decimal? EndCycleBirdBalance { get; set; }
}