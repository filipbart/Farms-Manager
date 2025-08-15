import type { GridColDef } from "@mui/x-data-grid";
import ActionsCell from "../../../components/datagrid/actions-cell";
import type { ProductionDataFlockLossListModel } from "../../../models/production-data/flock-loss";

interface GetFlockLossColumnsProps {
  setSelectedFlockLoss: (row: ProductionDataFlockLossListModel) => void;
  deleteFlockLoss: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
}

export const getFlockLossColumns = ({
  setSelectedFlockLoss,
  deleteFlockLoss,
  setIsEditModalOpen,
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
    },

    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 100,
      getActions: (params) => [
        <ActionsCell
          key="actions"
          params={params}
          onEdit={(row) => {
            setSelectedFlockLoss(row);
            setIsEditModalOpen(true);
          }}
          onDelete={deleteFlockLoss}
        />,
      ],
    },
  ];
};
