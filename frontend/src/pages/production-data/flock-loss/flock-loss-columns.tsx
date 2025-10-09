import type { GridCellParams, GridColDef } from "@mui/x-data-grid";
import ActionsCell from "../../../components/datagrid/actions-cell";
import type { ProductionDataFlockLossListModel } from "../../../models/production-data/flock-loss";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";

const getCellClassName = (
  params: GridCellParams,
  stats: { min: number; max: number; avg: number } | undefined,
  isLowerBetter: boolean
) => {
  if (!stats || params.value === null || params.value === undefined) return "";
  const value = params.value as number;
  const { avg } = stats;

  if (isLowerBetter) {
    if (value < avg * 0.95) return "cell-good";
    if (value > avg * 1.05) return "cell-bad";
  } else {
    if (value > avg * 1.05) return "cell-good";
    if (value < avg * 0.95) return "cell-bad";
  }

  return "cell-neutral";
};

interface GetFlockLossColumnsProps {
  setSelectedFlockLoss: (row: ProductionDataFlockLossListModel) => void;
  deleteFlockLoss: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  columnStats: any;
}

export const getFlockLossColumns = ({
  setSelectedFlockLoss,
  deleteFlockLoss,
  setIsEditModalOpen,
  columnStats,
}: GetFlockLossColumnsProps): GridColDef<ProductionDataFlockLossListModel>[] => {
  return [
    { field: "cycleText", headerName: "Identyfikator", width: 120 },
    { field: "farmName", headerName: "Ferma", width: 200 },
    { field: "hatcheryName", headerName: "Wylęgarnia", width: 150 },
    { field: "henhouseName", headerName: "Kurnik", width: 100 },
    {
      field: "insertionQuantity",
      headerName: "Ilość wstawiona",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 130,
    },

    // --- Pomiar I ---
    {
      field: "flockLoss1Day",
      headerName: "Doba I",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 100,
    },
    {
      field: "flockLoss1Quantity",
      headerName: "Upadki + wybrakowania I",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 100,
    },
    {
      field: "flockLoss1Percentage",
      headerName: "% Upadki + wybrakowania I",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 120,
      cellClassName: (params) =>
        getCellClassName(params, columnStats.flockLoss1Percentage, true),
    },

    // --- Pomiar II ---
    {
      field: "flockLoss2Day",
      headerName: "Doba II",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 100,
    },
    {
      field: "flockLoss2Quantity",
      headerName: "Upadki + wybrakowania II",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 100,
    },
    {
      field: "flockLoss2Percentage",
      headerName: "% Upadki + wybrakowania II",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 120,
      cellClassName: (params) =>
        getCellClassName(params, columnStats.flockLoss2Percentage, true),
    },

    // --- Pomiar III ---
    {
      field: "flockLoss3Day",
      headerName: "Doba III",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 100,
    },
    {
      field: "flockLoss3Quantity",
      headerName: "Upadki + wybrakowania III",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 100,
    },
    {
      field: "flockLoss3Percentage",
      headerName: "% Upadki + wybrakowania III",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 120,
      cellClassName: (params) =>
        getCellClassName(params, columnStats.flockLoss3Percentage, true),
    },

    // --- Pomiar IV ---
    {
      field: "flockLoss4Day",
      headerName: "Doba IV",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 100,
    },
    {
      field: "flockLoss4Quantity",
      headerName: "Upadki + wybrakowania IV",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 100,
    },
    {
      field: "flockLoss4Percentage",
      headerName: "% Upadki + wybrakowania IV",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 120,
      cellClassName: (params) =>
        getCellClassName(params, columnStats.flockLoss4Percentage, true),
    },

    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 100,
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return [
          <ActionsCell
            key="actions"
            params={params}
            onEdit={(row) => {
              setSelectedFlockLoss(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteFlockLoss}
          />,
        ];
      },
    },
  ];
};
