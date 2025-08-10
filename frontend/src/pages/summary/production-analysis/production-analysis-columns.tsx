import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { ProductionAnalysisRowModel } from "../../../models/summary/production-analysis";

export const getProductionAnalysisColumns =
  (): GridColDef<ProductionAnalysisRowModel>[] => {
    return [
      { field: "cycleText", headerName: "Identyfikator", flex: 1 },
      { field: "farmName", headerName: "Ferma", flex: 1 },
      { field: "henhouseName", headerName: "Kurnik", flex: 1 },
      {
        field: "insertionDate",
        headerName: "Data wstawienia",
        flex: 1,
        type: "string",
        valueGetter: (value: string) => {
          return value ? dayjs(value).format("YYYY-MM-DD") : "";
        },
      },
    ];
  };
