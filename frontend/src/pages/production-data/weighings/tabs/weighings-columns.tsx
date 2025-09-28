import { Box, Typography } from "@mui/material";
import type { GridColDef, GridRenderCellParams } from "@mui/x-data-grid";
import type { ProductionDataWeighingListModel } from "../../../../models/production-data/weighings";
import ActionsCell from "../../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";

interface GetWeighingsColumnsProps {
  setSelectedWeighing: (row: ProductionDataWeighingListModel) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
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
}: GetWeighingsColumnsProps): GridColDef<ProductionDataWeighingListModel>[] => {
  return [
    { field: "cycleText", headerName: "Identyfikator", width: 120 },
    { field: "farmName", headerName: "Ferma", width: 200 },
    { field: "hatcheryName", headerName: "Wylęgarnia", width: 100 },
    { field: "henhouseName", headerName: "Kurnik", width: 100 },
    {
      field: "weighing1Day",
      headerName: "Ważenie I (doba)",
      width: 140,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "weighing1Weight",
      headerName: "Masa ciała",

      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing1Weight", "weighing1Deviation"),
    },
    {
      field: "weighing2Day",
      headerName: "Ważenie II (doba)",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 140,
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
    },
    {
      field: "weighing3Day",
      headerName: "Ważenie III (doba)",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 140,
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
    },
    {
      field: "weighing4Day",
      headerName: "Ważenie IV (doba)",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 140,
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
    },
    {
      field: "weighing5Day",
      headerName: "Ważenie V (doba)",
      type: "number",
      headerAlign: "left",
      align: "left",
      width: 140,
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
};
