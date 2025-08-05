import type { GridColDef } from "@mui/x-data-grid";
import type { GasConsumptionListModel } from "../../../models/gas/gas-consumptions";
import ActionsCell from "../../../components/datagrid/actions-cell";

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
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "cost",
      headerName: "Koszt gazu [zł]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 200,
      getActions: (params) => [
        <ActionsCell
          key="actions"
          params={params}
          onEdit={(row) => {
            setSelectedGasConsumption(row);
            setIsEditModalOpen(true);
          }}
          onDelete={deleteGasConsumption}
        />,
      ],
    },
  ];
};
