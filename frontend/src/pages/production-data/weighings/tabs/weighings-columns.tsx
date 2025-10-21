import { Box, Typography } from "@mui/material";
import type {
  GridCellParams,
  GridColDef,
  GridRenderCellParams,
} from "@mui/x-data-grid";
import type { ProductionDataWeighingListModel } from "../../../../models/production-data/weighings";
import ActionsCell from "../../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { getAuditColumns } from "../../../../utils/audit-columns-helper";

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

interface GetWeighingsColumnsProps {
  setSelectedWeighing: (row: ProductionDataWeighingListModel) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  isAdmin?: boolean;
  columnStats: any;
}

const renderWeightCell = (
  params: GridRenderCellParams<ProductionDataWeighingListModel>,
  weightField: keyof ProductionDataWeighingListModel,
  deviationField: keyof ProductionDataWeighingListModel
) => {
  const weight = params.row[weightField] as number | undefined;
  const deviation = params.row[deviationField] as number | undefined;

  if (weight == null) return null;

  const deviationText =
    deviation != null ? ` (${deviation > 0 ? "+" : ""}${deviation})` : "";

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center", // Wyśrodkowanie w pionie
        height: "100%",
      }}
    >
      <Typography variant="body2" sx={{ whiteSpace: "nowrap" }}>
        {weight}
        {deviationText}
      </Typography>
    </Box>
  );
};

export const getWeighingsColumns = ({
  setSelectedWeighing,
  setIsEditModalOpen,
  isAdmin = false,
  columnStats,
}: GetWeighingsColumnsProps): GridColDef<ProductionDataWeighingListModel>[] => {
  const baseColumns: GridColDef[] = [
    { field: "cycleText", headerName: "Identyfikator", width: 120 },
    { field: "farmName", headerName: "Ferma", width: 200 },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
    { field: "hatcheryName", headerName: "Wylęgarnia", width: 100 },
    { field: "henhouseName", headerName: "Kurnik", width: 100 },
    {
      field: "weighing1Day",
      headerName: "Ważenie I (doba)",
      width: 140,
      type: "number",
      headerAlign: "left",
      align: "left",
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing1Day, true),
    },
    {
      field: "weighing1Weight",
      headerName: "Masa ciała",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing1Weight", "weighing1Deviation"),
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing1Weight, false),
    },
    {
      field: "weighing2Day",
      headerName: "Ważenie II (doba)",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 140,
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing2Day, true),
    },
    {
      field: "weighing2Weight",
      headerName: "Masa ciała",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing2Weight", "weighing2Deviation"),
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing2Weight, false),
    },
    {
      field: "weighing3Day",
      headerName: "Ważenie III (doba)",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 140,
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing3Day, true),
    },
    {
      field: "weighing3Weight",
      headerName: "Masa ciała",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing3Weight", "weighing3Deviation"),
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing3Weight, false),
    },
    {
      field: "weighing4Day",
      headerName: "Ważenie IV (doba)",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 140,
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing4Day, true),
    },
    {
      field: "weighing4Weight",
      headerName: "Masa ciała",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing4Weight", "weighing4Deviation"),
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing4Weight, false),
    },
    {
      field: "weighing5Day",
      headerName: "Ważenie V (doba)",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 140,
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing5Day, true),
    },
    {
      field: "weighing5Weight",
      headerName: "Masa ciała",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing5Weight", "weighing5Deviation"),
      cellClassName: (params) =>
        getCellClassName(params, columnStats.weighing5Weight, false),
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 200,
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return [
          <ActionsCell
            key="actions"
            params={params}
            onEdit={(row) => {
              setSelectedWeighing(row);
              setIsEditModalOpen(true);
            }}
          />,
        ];
      },
    },
  ];

  const auditColumns = getAuditColumns<ProductionDataWeighingListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
