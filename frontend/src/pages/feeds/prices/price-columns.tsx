import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { getAuditColumns } from "../../../utils/audit-columns-helper";
import type { FeedPriceListModel } from "../../../models/feeds/prices/feed-price";

export const getFeedsPricesColumns = ({
  setSelectedFeedPrice,
  deleteFeedPrice,
  setIsEditModalOpen,
  isAdmin = false,
}: {
  setSelectedFeedPrice: (s: any) => void;
  deleteFeedPrice: (id: string) => void;
  setIsEditModalOpen: (v: boolean) => void;
  isAdmin?: boolean;
}): GridColDef[] => {
  const baseColumns: GridColDef[] = [
    { field: "cycleText", headerName: "Identyfikator", flex: 1 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    {
      field: "priceDate",
      headerName: "Data publikacji",
      flex: 1,
      type: "string",
      valueGetter: (date: any) => {
        return date ? dayjs(date).format("YYYY-MM-DD") : "";
      },
    },
    { field: "name", headerName: "Typ (nazwa) paszy", flex: 1 },
    {
      field: "price",
      headerName: "Cena [zł]",
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
              setSelectedFeedPrice(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteFeedPrice}
          />,
        ];
      },
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];

  const auditColumns = getAuditColumns<FeedPriceListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
