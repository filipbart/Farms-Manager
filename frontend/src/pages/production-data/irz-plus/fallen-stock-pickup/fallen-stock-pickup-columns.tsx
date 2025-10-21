import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { getAuditColumns } from "../../../../utils/audit-columns-helper";
export const getFallenStockPickupColumns = ({
  setSelectedPickup,
  deletePickupRecord,
  setIsEditModalOpen,
  isAdmin = false,
}: {
  setSelectedPickup: (s: any) => void;
  deletePickupRecord: (id: string) => void;
  setIsEditModalOpen: (v: boolean) => void;
  isAdmin?: boolean;
}): GridColDef[] => {
  const baseColumns: GridColDef[] = [
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "cycleText", headerName: "Identyfikator cyklu", flex: 1 },
    {
      field: "date",
      headerName: "Data odbioru",
      flex: 1,
      type: "date",
      valueGetter: (value: string) => (value ? dayjs(value).toDate() : null),
    },
    {
      field: "quantity",
      headerName: "Sztuki odebrane",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
      aggregable: true,
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 150,
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return [
          <ActionsCell
            key="actions"
            params={params}
            onEdit={(row) => {
              setSelectedPickup(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deletePickupRecord}
          />,
        ];
      },
    },
  ];
  
  const auditColumns = getAuditColumns<any>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
