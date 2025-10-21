import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { HatcheryPriceListModel } from "../../models/hatcheries/hatcheries-prices";
import ActionsCell from "../../components/datagrid/actions-cell";
import { CommentCell } from "../../components/datagrid/comment-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { getAuditColumns } from "../../utils/audit-columns-helper";

export const getHatcheriesPriceColumns = ({
  setSelectedHatcheryPrice,
  setIsEditModalOpen,
  deleteHatcheryPrice,
  isAdmin = false,
}: {
  setSelectedHatcheryPrice: (price: HatcheryPriceListModel) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  deleteHatcheryPrice: (id: string) => void;
  isAdmin?: boolean;
}): GridColDef<HatcheryPriceListModel>[] => {
  const baseColumns: GridColDef<HatcheryPriceListModel>[] = [
    { field: "hatcheryName", headerName: "Wylęgarnia", flex: 1 },
    {
      field: "date",
      headerName: "Data",
      flex: 1,
      type: "string",
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD") : "";
      },
    },
    {
      field: "price",
      headerName: "Cena [zł]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "comment",
      headerName: "Komentarz",
      flex: 1,
      aggregable: false,
      renderCell: (params) => <CommentCell value={params.value} />,
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
              setSelectedHatcheryPrice(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteHatcheryPrice}
          />,
        ];
      },
    },
  ];
  
  const auditColumns = getAuditColumns<HatcheryPriceListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
