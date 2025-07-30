import { Button, Typography } from "@mui/material";
import type { GridColDef, GridRenderCellParams } from "@mui/x-data-grid";
import type { ProductionDataWeighingListModel } from "../../../../models/production-data/weighings";

interface GetWeighingsColumnsProps {
  setSelectedWeighing: (row: ProductionDataWeighingListModel) => void;
  deleteWeighing: (id: string) => void;
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
    <Typography variant="body2" sx={{ whiteSpace: "nowrap" }}>
      {weight}
      {deviationText}
    </Typography>
  );
};

export const getWeighingsColumns = ({
  setSelectedWeighing,
  deleteWeighing,
  setIsEditModalOpen,
}: GetWeighingsColumnsProps): GridColDef<ProductionDataWeighingListModel>[] => {
  return [
    { field: "cycleText", headerName: "Identyfikator", width: 120 },
    { field: "farmName", headerName: "Ferma", flex: 1.5 },
    { field: "hatcheryName", headerName: "Wylęgarnia", flex: 1 },
    { field: "henhouseName", headerName: "Kurnik", width: 100 },
    {
      field: "weighing1Day",
      headerName: "Ważenie I (doba)",
      type: "number",
      width: 140,
    },
    {
      field: "weighing1Weight",
      headerName: "Masa ciała",
      type: "number",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing1Weight", "weighing1Deviation"),
    },
    {
      field: "weighing2Day",
      headerName: "Ważenie II (doba)",
      type: "number",
      width: 140,
    },
    {
      field: "weighing2Weight",
      headerName: "Masa ciała",
      type: "number",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing2Weight", "weighing2Deviation"),
    },
    {
      field: "weighing3Day",
      headerName: "Ważenie III (doba)",
      type: "number",
      width: 140,
    },
    {
      field: "weighing3Weight",
      headerName: "Masa ciała",
      type: "number",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing3Weight", "weighing3Deviation"),
    },
    {
      field: "weighing4Day",
      headerName: "Ważenie IV (doba)",
      type: "number",
      width: 140,
    },
    {
      field: "weighing4Weight",
      headerName: "Masa ciała",
      type: "number",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing4Weight", "weighing4Deviation"),
    },
    {
      field: "weighing5Day",
      headerName: "Ważenie V (doba)",
      type: "number",
      width: 140,
    },
    {
      field: "weighing5Weight",
      headerName: "Masa ciała",
      type: "number",
      width: 120,
      renderCell: (params) =>
        renderWeightCell(params, "weighing5Weight", "weighing5Deviation"),
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 200,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedWeighing(params.row);
            setIsEditModalOpen(true);
          }}
        >
          Edytuj
        </Button>,
        <Button
          key="delete"
          variant="outlined"
          size="small"
          color="error"
          onClick={() => {
            deleteWeighing(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
  ];
};
