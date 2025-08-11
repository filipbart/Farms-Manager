import dayjs from "dayjs";
import type { ProductionAnalysisRowModel } from "../../../models/summary/production-analysis";
import type {
  GridColDef,
  GridRenderCellParams,
} from "@mui/x-data-grid-premium";

const renderAvgWeightCell = (
  params: GridRenderCellParams<any, ProductionAnalysisRowModel>,
  avgWeightField: keyof ProductionAnalysisRowModel,
  deviationField: keyof ProductionAnalysisRowModel
) => {
  const avgWeight = params.row[avgWeightField] as number | null;
  const deviation = params.row[deviationField] as number | null;

  if (avgWeight === null || avgWeight === undefined) {
    return "";
  }

  const formattedAvgWeight = avgWeight.toFixed(3);

  if (deviation === null || deviation === undefined || deviation === 0) {
    return formattedAvgWeight;
  }

  const deviationColor = deviation > 0 ? "green" : "red";
  const sign = deviation > 0 ? "+" : "";

  return (
    <div>
      {formattedAvgWeight}
      <span
        style={{ color: deviationColor, marginLeft: "8px", fontWeight: "bold" }}
      >
        ({sign}
        {deviation.toFixed(0)})
      </span>
    </div>
  );
};

export const getProductionAnalysisColumns =
  (): GridColDef<ProductionAnalysisRowModel>[] => {
    return [
      // --- Dane Podstawowe ---
      { field: "cycleText", headerName: "Cykl", width: 90 },
      { field: "farmName", headerName: "Ferma", flex: 1, minWidth: 150 },
      { field: "henhouseName", headerName: "Kurnik", flex: 1, minWidth: 120 },
      {
        field: "hatcheryName",
        headerName: "Wylęgarnia",
        flex: 1,
        minWidth: 150,
      },
      {
        field: "insertionDate",
        headerName: "Data wstawienia",
        width: 130,
        valueFormatter: (value: any) =>
          value ? dayjs(value).format("YYYY-MM-DD") : "",
      },
      {
        field: "insertionQuantity",
        headerName: "Sztuki wstawione",
        type: "number",
        width: 130,
      },

      // --- Sprzedaż Częściowa (Ubiórka) ---
      {
        field: "partSaleDate",
        headerName: "Data ubiórki",
        width: 130,
        valueFormatter: (value: any) =>
          value ? dayjs(value).format("YYYY-MM-DD") : "",
      },
      {
        field: "partSaleSoldCount",
        headerName: "Ubiórka [szt]",
        type: "number",
        width: 120,
      },
      {
        field: "partSaleSoldWeight",
        headerName: "Ubiórka [kg]",
        type: "number",
        width: 120,
      },
      {
        field: "partSaleAvgWeight",
        headerName: "Śr. waga ubiórka [kg] (odchył)",
        type: "number",
        width: 180,
        renderCell: (
          params: GridRenderCellParams<any, ProductionAnalysisRowModel>
        ) =>
          renderAvgWeightCell(
            params,
            "partSaleAvgWeight",
            "partSaleAvgWeightDeviation"
          ),
      },
      {
        field: "partSaleFarmerWeight",
        headerName: "Waga hodowcy ubiórka [kg]",
        type: "number",
        width: 150,
      },
      {
        field: "partSaleAgeInDays",
        headerName: "Wiek ubiórki [dni]",
        type: "number",
        width: 130,
      },
      {
        field: "partSaleWeightDiffPct",
        headerName: "Różnica wag ubiórka [%]",
        type: "number",
        width: 150,
        valueGetter: (value: any) => (value ? `${value.toFixed(2)}%` : ""),
      },
      {
        field: "partSaleDeadCount",
        headerName: "Martwe ubiórka [szt]",
        type: "number",
        width: 140,
      },
      {
        field: "partSaleDeadWeight",
        headerName: "Martwe ubiórka [kg]",
        type: "number",
        width: 140,
      },
      {
        field: "partSaleConfiscatedCount",
        headerName: "Konfiskata ubiórka [szt]",
        type: "number",
        width: 150,
      },
      {
        field: "partSaleConfiscatedWeight",
        headerName: "Konfiskata ubiórka [kg]",
        type: "number",
        width: 150,
      },
      {
        field: "partSaleSettlementCount",
        headerName: "Ubiórka do rozliczenia [szt]",
        type: "number",
        width: 160,
      },
      {
        field: "partSaleSettlementWeight",
        headerName: "Ubiórka do rozliczenia [kg]",
        type: "number",
        width: 160,
      },

      // --- Sprzedaż Całkowita ---
      {
        field: "totalSaleDate",
        headerName: "Data sprzedaży całk.",
        width: 130,
        valueFormatter: (value: any) =>
          value ? dayjs(value).format("YYYY-MM-DD") : "",
      },
      {
        field: "totalSaleSoldCount",
        headerName: "Sprzedaż całk. [szt]",
        type: "number",
        width: 130,
      },
      {
        field: "totalSaleSoldWeight",
        headerName: "Sprzedaż całk. [kg]",
        type: "number",
        width: 130,
      },
      {
        field: "totalSaleAvgWeight",
        headerName: "Śr. waga całk. [kg] (odchył)",
        type: "number",
        width: 180,
        renderCell: (
          params: GridRenderCellParams<any, ProductionAnalysisRowModel>
        ) =>
          renderAvgWeightCell(
            params,
            "totalSaleAvgWeight",
            "totalSaleAvgWeightDeviation"
          ),
      },
      {
        field: "totalSaleFarmerWeight",
        headerName: "Waga hodowcy całk. [kg]",
        type: "number",
        width: 150,
      },
      {
        field: "totalSaleAgeInDays",
        headerName: "Wiek sprzedaży całk. [dni]",
        type: "number",
        width: 150,
      },
      {
        field: "totalWeightDiffPct",
        headerName: "Różnica wag całk. [%]",
        type: "number",
        width: 150,
        valueGetter: (value: any) => (value ? `${value.toFixed(2)}%` : ""),
      },
      {
        field: "totalSaleDeadCount",
        headerName: "Martwe całk. [szt]",
        type: "number",
        width: 140,
      },
      {
        field: "totalSaleDeadWeight",
        headerName: "Martwe całk. [kg]",
        type: "number",
        width: 140,
      },
      {
        field: "totalSaleConfiscatedCount",
        headerName: "Konfiskata całk. [szt]",
        type: "number",
        width: 150,
      },
      {
        field: "totalSaleConfiscatedWeight",
        headerName: "Konfiskata całk. [kg]",
        type: "number",
        width: 150,
      },
      {
        field: "totalSaleSettlementCount",
        headerName: "Całk. do rozliczenia [szt]",
        type: "number",
        width: 160,
      },
      {
        field: "totalSaleSettlementWeight",
        headerName: "Całk. do rozliczenia [kg]",
        type: "number",
        width: 160,
      },

      // --- Podsumowanie Łączne ---
      {
        field: "combinedSoldCount",
        headerName: "Sprzedane łącznie [szt]",
        type: "number",
        width: 160,
      },
      {
        field: "combinedSoldWeight",
        headerName: "Sprzedane łącznie [kg]",
        type: "number",
        width: 160,
      },
      {
        field: "combinedAvgWeight",
        headerName: "Śr. waga łączna [kg]",
        type: "number",
        width: 140,
      },
      {
        field: "combinedFarmerWeight",
        headerName: "Waga hodowcy łączna [kg]",
        type: "number",
        width: 160,
      },
      {
        field: "combinedSettlementCount",
        headerName: "Łącznie do rozliczenia [szt]",
        type: "number",
        width: 170,
      },
      {
        field: "combinedSettlementWeight",
        headerName: "Łącznie do rozliczenia [kg]",
        type: "number",
        width: 170,
      },
      {
        field: "combinedAvgAgeInDays",
        headerName: "Śr. wiek łączny [dni]",
        type: "number",
        width: 140,
      },

      // --- Statystyki Cyklu ---
      {
        field: "deadCountCycle",
        headerName: "Padłe w cyklu [szt]",
        type: "number",
        width: 140,
      },
      {
        field: "defectiveCountCycle",
        headerName: "Wybrakowane w cyklu [szt]",
        type: "number",
        width: 150,
      },
      {
        field: "survivalRatePct",
        headerName: "Przeżywalność [%]",
        type: "number",
        width: 130,
        valueGetter: (value: any) => (value ? `${value.toFixed(2)}%` : ""),
      },
      {
        field: "deadPctCycle",
        headerName: "Padłe w cyklu [%]",
        type: "number",
        width: 130,
        valueGetter: (value: any) => (value ? `${value.toFixed(2)}%` : ""),
      },
      {
        field: "defectivePctCycle",
        headerName: "Wybrakowane w cyklu [%]",
        type: "number",
        width: 150,
        valueGetter: (value: any) => (value ? `${value.toFixed(2)}%` : ""),
      },
      {
        field: "deadAndDefectivePctCycle",
        headerName: "Padłe i wybrakowane [%]",
        type: "number",
        width: 160,
        valueGetter: (value: any) => (value ? `${value.toFixed(2)}%` : ""),
      },

      // --- Wskaźniki Produkcyjne ---
      {
        field: "feedConsumedTons",
        headerName: "Pasza [t]",
        type: "number",
        width: 100,
      },
      {
        field: "fcrWithLosses",
        headerName: "FCR (z padłymi)",
        type: "number",
        width: 130,
      },
      {
        field: "fcrWithoutLosses",
        headerName: "FCR (do rozliczenia)",
        type: "number",
        width: 140,
      },
      {
        field: "points",
        headerName: "Punkty",
        type: "number",
        width: 100,
      },
      {
        field: "eww",
        headerName: "EWW",
        type: "number",
        width: 100,
      },

      // --- Powierzchnia i Gaz ---
      {
        field: "houseAreaM2",
        headerName: "Powierzchnia [m²]",
        type: "number",
        width: 130,
      },
      {
        field: "kgPerM2BeforeConf",
        headerName: "Kg/m² (przed konf.)",
        type: "number",
        width: 150,
      },
      {
        field: "kgPerM2AfterConf",
        headerName: "Kg/m² (po konf.)",
        type: "number",
        width: 140,
      },
      {
        field: "gasConsumptionLiters",
        headerName: "Zużycie gazu [L]",
        type: "number",
        width: 130,
      },
      {
        field: "gasConsumptionPerM2",
        headerName: "Gaz/m² [L]",
        type: "number",
        width: 110,
      },

      // --- Bilans ---
      {
        field: "endCycleBirdBalance",
        headerName: "Bilans sztuk",
        type: "number",
        width: 120,
      },
    ];
  };
