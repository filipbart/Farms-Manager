import type { GridColDef, GridColumnGroupingModel } from "@mui/x-data-grid";
import type { ProductionDataTransferFeedListModel } from "../../../models/production-data/transfer-feed";
import dayjs from "dayjs";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";

interface GetFeedTransfersColumnsProps {
  setSelectedTransfer: (row: ProductionDataTransferFeedListModel) => void;
  deleteTransfer: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
}

export const getFeedTransfersColumns = ({
  setSelectedTransfer,
  deleteTransfer,
  setIsEditModalOpen,
}: GetFeedTransfersColumnsProps): {
  columns: GridColDef<ProductionDataTransferFeedListModel>[];
  columnGroupingModel: GridColumnGroupingModel;
} => {
  const columns: GridColDef<ProductionDataTransferFeedListModel>[] = [
    // Kolumny dla grupy "Z"
    { field: "fromCycleText", headerName: "Identyfikator", flex: 1 },
    { field: "fromFarmName", headerName: "Ferma", flex: 1.5 },
    { field: "fromHenhouseName", headerName: "Kurnik", flex: 0.8 },
    // Kolumny dla grupy "Do"
    { field: "toCycleText", headerName: "Identyfikator", flex: 1 },
    { field: "toFarmName", headerName: "Ferma", flex: 1.5 },
    { field: "toHenhouseName", headerName: "Kurnik", flex: 0.8 },
    // Pozostałe kolumny
    { field: "feedName", headerName: "Typ (nazwa) paszy", flex: 1 },
    {
      field: "tonnage",
      headerName: "Tonaż pozostały [t]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "value",
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
              setSelectedTransfer(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteTransfer}
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

  const columnGroupingModel: GridColumnGroupingModel = [
    {
      groupId: "Pasza przeniesiona Z",
      children: [
        { field: "fromCycleText" },
        { field: "fromFarmName" },
        { field: "fromHenhouseName" },
      ],
    },
    {
      groupId: "Pasza przeniesiona DO",
      children: [
        { field: "toCycleText" },
        { field: "toFarmName" },
        { field: "toHenhouseName" },
      ],
    },
  ];

  return { columns, columnGroupingModel };
};
