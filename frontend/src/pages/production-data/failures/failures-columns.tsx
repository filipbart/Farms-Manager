import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { ProductionDataFailureListModel } from "../../../models/production-data/failures";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { getAuditColumns } from "../../../utils/audit-columns-helper";

interface GetProductionDataFailuresColumnsProps {
  setSelectedFailure: (row: ProductionDataFailureListModel) => void;
  deleteFailure: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  isAdmin?: boolean;
}

export const getProductionDataFailuresColumns = ({
  setSelectedFailure,
  deleteFailure,
  setIsEditModalOpen,
  isAdmin = false,
}: GetProductionDataFailuresColumnsProps): GridColDef<ProductionDataFailureListModel>[] => {
  const baseColumns: GridColDef[] = [
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
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "defectiveCount",
      headerName: "Wybrakowania [szt.]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
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
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return [
          <ActionsCell
            key="actions"
            params={params}
            onEdit={(row) => {
              setSelectedFailure(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteFailure}
          />,
        ];
      },
    },
  ];

  const auditColumns = getAuditColumns<ProductionDataFailureListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
