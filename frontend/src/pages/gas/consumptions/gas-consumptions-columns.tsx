import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { getAuditColumns } from "../../../utils/audit-columns-helper";
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
  isAdmin = false,
}: {
  setSelectedGasConsumption: (s: any) => void;
  deleteGasConsumption: (id: string) => void;
  setIsEditModalOpen: (v: boolean) => void;
  isAdmin?: boolean;
}): GridColDef[] => {
  const baseColumns: GridColDef[] = [
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
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return [
          <ActionsCell
            key="actions"
            params={params}
            onEdit={(row) => {
              setSelectedGasConsumption(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteGasConsumption}
          />,
        ];
      },
    },
  ];

  const auditColumns = getAuditColumns<GasConsumptionListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
