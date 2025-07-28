import { Button } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { ProductionDataFailureListModel } from "../../../models/production-data/failures/failures";

interface GetProductionDataFailuresColumnsProps {
  setSelectedFailure: (row: ProductionDataFailureListModel) => void;
  deleteFailure: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
}

export const getProductionDataFailuresColumns = ({
  setSelectedFailure,
  deleteFailure,
  setIsEditModalOpen,
}: GetProductionDataFailuresColumnsProps): GridColDef<ProductionDataFailureListModel>[] => {
  return [
    {
      field: "id",
      headerName: "Id",
      width: 70,
    },
    {
      field: "cycleText",
      headerName: "Cykl",
      flex: 1,
    },
    {
      field: "farmName",
      headerName: "Ferma",
      flex: 1,
    },
    {
      field: "henhouseName",
      headerName: "Kurnik",
      flex: 1,
    },
    {
      field: "deadCount",
      headerName: "Upadki [szt.]",
      flex: 1,
    },
    {
      field: "defectiveCount",
      headerName: "Wybrakowania [szt.]",
      flex: 1,
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      flex: 1,
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD HH:mm") : "";
      },
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
            setSelectedFailure(params.row);
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
            deleteFailure(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
  ];
};
