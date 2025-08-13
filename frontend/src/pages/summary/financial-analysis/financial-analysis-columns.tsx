import type { GridColDef } from "@mui/x-data-grid-premium";
import dayjs from "dayjs";
import type { FinancialAnalysisRowModel } from "../../../models/summary/financial-analysis";

// --- Funkcje pomocnicze do formatowania ---

const formatDate = (value: string | null | undefined): string => {
  if (!value) return "";
  return dayjs(value).format("YYYY-MM-DD");
};

const polishFormatter = (value: number | null | undefined): string => {
  if (typeof value !== "number") return "";
  return new Intl.NumberFormat("pl-PL", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(value);
};

const polishFormatterWeight = (value: number | null | undefined): string => {
  if (typeof value !== "number") return "";
  return new Intl.NumberFormat("pl-PL", {
    minimumFractionDigits: 3,
    maximumFractionDigits: 3,
  }).format(value);
};

export const getFinancialAnalysisColumns =
  (): GridColDef<FinancialAnalysisRowModel>[] => {
    return [
      // --- Dane podstawowe ---
      { field: "cycleText", headerName: "Identyfikator", width: 110 },
      { field: "farmName", headerName: "Ferma", width: 180 },
      { field: "henhouseName", headerName: "Kurnik", width: 150 },
      { field: "hatcheryName", headerName: "Wylęgarnia", width: 180 },
      {
        field: "insertionDate",
        headerName: "Data wstawienia",
        width: 130,
        valueGetter: formatDate,
      },

      // --- Sprzedaż częściowa (Ubiórka) ---
      {
        field: "partSaleDate",
        headerName: "Data sprzedaży ubiórka",
        width: 140,
        valueGetter: formatDate,
      },
      {
        field: "partSaleBasePrice",
        headerName: "Cena bazowa ubiórka [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "partSaleFinalPrice",
        headerName: "Cena ostateczna ubiórka [zł]",
        width: 160,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "partSaleSettlementWeight",
        headerName: "Masa rozliczeniowa ubiórka [kg]",
        width: 200,
        type: "number",
        valueGetter: polishFormatterWeight,
      },
      {
        field: "partSaleRevenue",
        headerName: "Przychód ubiórka [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },

      // --- Sprzedaż całkowita ---
      {
        field: "totalSaleDate",
        headerName: "Data sprzedaży całkowitej",
        width: 140,
        valueGetter: formatDate,
      },
      {
        field: "totalSaleBasePrice",
        headerName: "Cena bazowa całk. [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "totalSaleFinalPrice",
        headerName: "Cena ostateczna całk. [zł]",
        width: 160,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "totalSaleSettlementWeight",
        headerName: "Masa rozliczeniowa całk. [kg]",
        width: 200,
        type: "number",
        valueGetter: polishFormatterWeight,
      },
      {
        field: "totalSaleRevenue",
        headerName: "Przychód całk. [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },

      // --- Podsumowanie ---
      {
        field: "combinedSettlementWeight",
        headerName: "Masa rozliczeniowa razem [kg]",
        width: 200,
        type: "number",
        valueGetter: polishFormatterWeight,
      },
      {
        field: "combinedRevenue",
        headerName: "Przychód razem [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "combinedRevenueVat",
        headerName: "VAT z przychodu [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "revenuePerKgLiveWeight",
        headerName: "Przychód na kg żywca [zł]",
        width: 160,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "henhouseAreaM2",
        headerName: "Powierzchnia kurnika [m2]",
        width: 180,
        type: "number",
        valueGetter: polishFormatter,
      },

      // --- Koszty ---
      {
        field: "feedCost",
        headerName: "Koszt zużytej paszy [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "chicksCost",
        headerName: "Koszt zakupu piskląt [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "vetCareCost",
        headerName: "Koszt obsługi wet. [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "gasCost",
        headerName: "Koszt gazu [zł]",
        width: 120,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "otherCosts",
        headerName: "Koszty pozostałe [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "totalCosts",
        headerName: "Koszty razem [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "vatCosts",
        headerName: "VAT z kosztów [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "income",
        headerName: "Dochód [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },

      // --- Wskaźniki na kg żywca ---
      {
        field: "feedCostPerKgLiveWeight",
        headerName: "Koszt paszy na kg [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "chicksCostPerKgLiveWeight",
        headerName: "Koszt piskląt na kg [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "vetCareCostPerKgLiveWeight",
        headerName: "Koszt wet. na kg [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "gasCostPerKgLiveWeight",
        headerName: "Koszt gazu na kg [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "otherCostsPerKgLiveWeight",
        headerName: "Koszty poz. na kg [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "totalCostsPerKgLiveWeight",
        headerName: "Koszty razem na kg [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "incomePerKgLiveWeight",
        headerName: "Dochód na kg żywca [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },

      // --- Wskaźniki na m2 ---
      {
        field: "feedCostPerM2",
        headerName: "Koszt paszy na m2 [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "chicksCostPerM2",
        headerName: "Koszt piskląt na m2 [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "vetCareCostPerM2",
        headerName: "Koszt wet. na m2 [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "gasCostPerM2",
        headerName: "Koszt gazu na m2 [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "otherCostsPerM2",
        headerName: "Koszty poz. na m2 [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "totalCostsPerM2",
        headerName: "Koszty razem na m2 [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "revenuePerM2",
        headerName: "Przychód na m2 [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
      {
        field: "incomePerM2",
        headerName: "Dochód na m2 [zł]",
        width: 150,
        type: "number",
        valueGetter: polishFormatter,
      },
    ];
  };
