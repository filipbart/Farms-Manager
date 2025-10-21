import dayjs from "dayjs";
import type { ProductionDataRemainingFeedListModel } from "../../../models/production-data/remaining-feed";
import type { GridColDef } from "@mui/x-data-grid";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { getAuditColumns } from "../../../utils/audit-columns-helper";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";

interface GetRemainingFeedColumnsProps {
  setSelectedRemainingFeed: (row: ProductionDataRemainingFeedListModel) => void;
  deleteRemainingFeed: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  isAdmin?: boolean;
}

export const getRemainingFeedColumns = ({
  setSelectedRemainingFeed,
  deleteRemainingFeed,
  setIsEditModalOpen,
  isAdmin = false,
}: GetRemainingFeedColumnsProps): GridColDef[] => {
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
      field: "feedName",
      headerName: "Typ (nazwa) paszy",
      flex: 1,
    },
    {
      field: "remainingTonnage",
      headerName: "Tonaż pozostały [t]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "remainingValue",
      headerName: "Wartość [zł]",
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
              setSelectedRemainingFeed(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteRemainingFeed}
          />,
        ];
      },
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      flex: 1,
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD HH:mm") : "";
      },
    },
  ];

  const auditColumns = getAuditColumns<ProductionDataRemainingFeedListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
