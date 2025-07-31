import { Button } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import type { GasConsumptionListModel } from "../../../models/gas/gas-consumptions";

interface GetGasConsumptionsColumnsProps {
  setSelectedGasConsumption: (row: GasConsumptionListModel) => void;
  deleteGasConsumption: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
}

export const getGasConsumptionsColumns = ({
  setSelectedGasConsumption,
  deleteGasConsumption,
  setIsEditModalOpen,
}: GetGasConsumptionsColumnsProps): GridColDef<GasConsumptionListModel>[] => {
  return [
    {
      field: "cycleText",
      headerName: "Identyfikator",
      flex: 1,
    },
    {
      field: "farmName",
      headerName: "Ferma",
      flex: 1.5,
    },
    {
      field: "quantityConsumed",
      headerName: "Ilość zużytego gazu [l]",
      flex: 1,
    },
    {
      field: "cost",
      headerName: "Koszt gazu [zł]",
      flex: 1,
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
            setSelectedGasConsumption(params.row);
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
            deleteGasConsumption(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
  ];
};
