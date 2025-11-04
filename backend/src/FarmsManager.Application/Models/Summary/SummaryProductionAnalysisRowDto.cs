namespace FarmsManager.Application.Models.Summary;

public class SummaryProductionAnalysisRowDto
{
    public int Id { get; set; }
    
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
    public decimal? PartSaleAvgWeight
    {
        get
        {
            if (PartSaleSoldCount is null or 0) return 0;
            return PartSaleSoldWeight / PartSaleSoldCount;
        }
    }

    /// <summary>
    /// Odchył normy masy ciała ubiórki dla średniej 
    /// </summary>
    public decimal? PartSaleAvgWeightDeviation { get; set; }

    /// <summary>Waga hodowcy ubiórka [kg]</summary>
    public decimal? PartSaleFarmerWeight { get; set; }

    /// <summary>Doba ubiórki</summary>
    public int? PartSaleAgeInDays { get; set; }

    /// <summary>Różnice wagowe ubiórka [%]</summary>
    public decimal? PartSaleWeightDiffPct
    {
        get
        {
            if (PartSaleFarmerWeight is null or 0) return 0;
            return (PartSaleFarmerWeight - PartSaleSoldWeight) / PartSaleFarmerWeight * 100;
        }
    }

    /// <summary>Kurczęta martwe ubiórka [szt]</summary>
    public int? PartSaleDeadCount { get; set; }

    /// <summary>Kurczęta martwe ubiórka [kg]</summary>
    public decimal? PartSaleDeadWeight { get; set; }

    /// <summary>Konfiskata ubiórka [szt]</summary>
    public int? PartSaleConfiscatedCount { get; set; }

    /// <summary>Konfiskata ubiórka [kg]</summary>
    public decimal? PartSaleConfiscatedWeight { get; set; }

    /// <summary>Liczba sztuk uwzględnionych do rozliczenia ubiórka</summary>
    public int? PartSaleSettlementCount => PartSaleSoldCount.GetValueOrDefault() - PartSaleDeadCount.GetValueOrDefault() - PartSaleConfiscatedCount.GetValueOrDefault();

    /// <summary>Masa ciała sprzedanych ptaków do rozliczenia ubióka [kg]</summary>
    public decimal? PartSaleSettlementWeight => PartSaleSoldWeight.GetValueOrDefault() - PartSaleDeadWeight.GetValueOrDefault() - PartSaleConfiscatedWeight.GetValueOrDefault();

    /// <summary>Data sprzedaży całkowitej</summary>
    public DateOnly? TotalSaleDate { get; set; }

    /// <summary>Sztuki sprzedane sprzedaż całkowita</summary>
    public int? TotalSaleSoldCount { get; set; }

    /// <summary>Masa sprzedanych ptaków Ubojnia sprzedaż całkowita</summary>
    public decimal? TotalSaleSoldWeight { get; set; }

    /// <summary>Śr. Masa ciała sprzedanych ptaków sprzedaż całkowita</summary>
    public decimal? TotalSaleAvgWeight
    {
        get
        {
            if (TotalSaleSoldCount is null or 0) return 0;
            return TotalSaleSoldWeight / TotalSaleSoldCount;
        }
    }

    /// <summary>
    /// Odchył normy masy ciała sprzedaży dla średniej
    /// </summary>
    public decimal? TotalSaleAvgWeightDeviation { get; set; }

    /// <summary>Waga hodowcy sprzedaż całkowita [kg]</summary>
    public decimal? TotalSaleFarmerWeight { get; set; }

    /// <summary>Doba sprzedaż całkowita</summary>
    public int? TotalSaleAgeInDays { get; set; }

    /// <summary>Różnice wagowe sprzedaż całkowita [%]</summary>
    public decimal? TotalWeightDiffPct
    {
        get
        {
            if (TotalSaleFarmerWeight is null or 0) return 0;
            return (TotalSaleFarmerWeight - TotalSaleSoldWeight) / TotalSaleFarmerWeight * 100;
        }
    }

    /// <summary>Kurczęta martwe sprzedaż całkowita [szt]</summary>
    public int? TotalSaleDeadCount { get; set; }

    /// <summary>Kurczęta martwe sprzedaż całkowita [kg]</summary>
    public decimal? TotalSaleDeadWeight { get; set; }

    /// <summary>Konfiskata sprzedaż całkowita [szt]</summary>
    public int? TotalSaleConfiscatedCount { get; set; }

    /// <summary>Konfiskata sprzedaż całkowita [kg]</summary>
    public decimal? TotalSaleConfiscatedWeight { get; set; }

    /// <summary>Liczba sztuk uwzględnionych do rozliczenia sprzedaż całkowita</summary>
    public int? TotalSaleSettlementCount => TotalSaleSoldCount.GetValueOrDefault() - TotalSaleDeadCount.GetValueOrDefault() - TotalSaleConfiscatedCount.GetValueOrDefault();

    /// <summary>Masa ciała sprzedanych ptaków do rozliczenia sprzedaż całkowita</summary>
    public decimal? TotalSaleSettlementWeight => TotalSaleSoldWeight.GetValueOrDefault() - TotalSaleDeadWeight.GetValueOrDefault() - TotalSaleConfiscatedWeight.GetValueOrDefault();

    /// <summary>Sztuki sprzedane razem</summary>
    public int? CombinedSoldCount { get; set; }

    /// <summary>Masa sprzedanych ptaków Ubojnia razem [kg]</summary>
    public decimal? CombinedSoldWeight { get; set; }

    /// <summary>Śr. Masa ciała sprzedanych ptaków razem [kg]</summary>
    public decimal? CombinedAvgWeight
    {
        get
        {
            if (CombinedSoldCount is null or 0) return 0;
            return CombinedSoldWeight / CombinedSoldCount;
        }
    }

    /// <summary>Waga hodowcy razem [kg]</summary>
    public decimal? CombinedFarmerWeight { get; set; }

    /// <summary>Liczba sztuk uwzględnionych do rozliczenia ubiórka + sprzedaż całkowita</summary>
    public int? CombinedSettlementCount => PartSaleSettlementCount.GetValueOrDefault() + TotalSaleSettlementCount.GetValueOrDefault();

    /// <summary>Masa ciała sprzedanych ptaków do rozliczenia ubiórka + sprzedaż całkowita [kg]</summary>
    public decimal? CombinedSettlementWeight => PartSaleSettlementWeight.GetValueOrDefault() + TotalSaleSettlementWeight.GetValueOrDefault();

    /// <summary>Średnia doba życia ubiórka + sprzedaż całkowita</summary>
    public decimal? CombinedAvgAgeInDays
    {
        get
        {
            if (CombinedSettlementCount is null or 0) return 0;

            var weightedSum = PartSaleSettlementCount.GetValueOrDefault() * PartSaleAgeInDays.GetValueOrDefault() +
                              TotalSaleSettlementCount.GetValueOrDefault() * TotalSaleAgeInDays.GetValueOrDefault();
                              
            return (decimal)weightedSum / CombinedSettlementCount;
        }
    }

    /// <summary>Sztuki padłe z cyklu produkcyjnego</summary>
    public int? DeadCountCycle { get; set; }

    /// <summary>Sztuki wybrakowane z cyklu produkcyjnego</summary>
    public int? DefectiveCountCycle { get; set; }

    /// <summary>Przeżywalność z konfiskatami i upadkami na ubojni [%]</summary>
    public decimal? SurvivalRatePct
    {
        get
        {
            if (InsertionQuantity is null or 0) return 0;
            
            var totalLosses = PartSaleDeadCount.GetValueOrDefault() +
                              PartSaleConfiscatedCount.GetValueOrDefault() +
                              TotalSaleDeadCount.GetValueOrDefault() +
                              TotalSaleConfiscatedCount.GetValueOrDefault() +
                              DeadCountCycle.GetValueOrDefault() +
                              DefectiveCountCycle.GetValueOrDefault();
            
            return (InsertionQuantity - totalLosses) * 100m / InsertionQuantity;
        }
    }

    /// <summary>% sztuk padłych z cyklu produkcyjnego</summary>
    public decimal? DeadPctCycle
    {
        get
        {
            if (InsertionQuantity is null or 0) return 0;
            return DeadCountCycle * 100m / InsertionQuantity;
        }
    }

    /// <summary>% sztuk wybrakowanych z cyklu produkcyjnego</summary>
    public decimal? DefectivePctCycle
    {
        get
        {
            if (InsertionQuantity is null or 0) return 0;
            return DefectiveCountCycle * 100m / InsertionQuantity;
        }
    }

    /// <summary>% sztuk padłych i wybrakowanych z cyklu produkcyjnego</summary>
    public decimal? DeadAndDefectivePctCycle
    {
        get
        {
            if (InsertionQuantity is null or 0) return 0;
            return (DeadCountCycle.GetValueOrDefault() + DefectiveCountCycle.GetValueOrDefault()) * 100m / InsertionQuantity;
        }
    }

    /// <summary>Spożyta pasza [t]</summary>
    public decimal? FeedConsumedTons { get; set; }

    /// <summary>FCR (z padłymi i konfiskatami)</summary>
    public decimal? FcrWithLosses
    {
        get
        {
            if (CombinedSoldWeight is null or 0) return 0;
            return FeedConsumedTons * 1000 / CombinedSoldWeight;
        }
    }

    /// <summary>FCR (bez padłych i konfiskat)</summary>
    public decimal? FcrWithoutLosses
    {
        get
        {
            if (CombinedSettlementWeight is null or 0) return 0;
            return FeedConsumedTons * 1000 / CombinedSettlementWeight;
        }
    }

    /// <summary>Punkty</summary>
    public decimal? Points => CombinedAvgWeight.GetValueOrDefault() - FcrWithoutLosses.GetValueOrDefault();

    /// <summary>EWW</summary>
    public decimal? Eww
    {
        get
        {
            var denominator = CombinedAvgAgeInDays * FcrWithoutLosses;
            if (denominator is null or 0) return 0;
            
            return SurvivalRatePct * CombinedAvgWeight / denominator * 100;
        }
    }

    /// <summary>Powierzchnia kurnika [m2]</summary>
    public decimal? HouseAreaM2 { get; set; }

    /// <summary>Liczba kg uzyskanych z m2 (przed konfiskatami)</summary>
    public decimal? KgPerM2BeforeConf
    {
        get
        {
            if (HouseAreaM2 is null or 0) return 0;
            return CombinedSoldWeight / HouseAreaM2;
        }
    }
    
    /// <summary>Liczba kg uzyskanych z m2 (po konfiskatach)</summary>
    public decimal? KgPerM2AfterConf
    {
        get
        {
            if (HouseAreaM2 is null or 0) return 0;
            return CombinedSettlementWeight / HouseAreaM2;
        }
    }

    /// <summary>Zużycie gazu [L]</summary>
    public decimal? GasConsumptionLiters { get; set; }

    /// <summary>Zużycie gazu na 1m2 powierzchni</summary>
    public decimal? GasConsumptionPerM2
    {
        get
        {
            if (HouseAreaM2 is null or 0) return 0;
            return GasConsumptionLiters / HouseAreaM2;
        }
    }

    /// <summary>Bilans sztuk na koniec cyklu</summary>
    public int? EndCycleBirdBalance
    {
        get
        {
            var birdsAccountedFor = PartSaleSoldCount.GetValueOrDefault() +
                                    TotalSaleSoldCount.GetValueOrDefault() +
                                    DeadCountCycle.GetValueOrDefault() +
                                    DefectiveCountCycle.GetValueOrDefault();

            return birdsAccountedFor - InsertionQuantity.GetValueOrDefault();
        }
    }
}