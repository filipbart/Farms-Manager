import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../../components/datagrid/actions-cell";

export const getFeedsPriceColumns = ({
  setSelectedFeedPrice,
  setIsEditModalOpen,
  deleteFeedPrice,
}: {
  setSelectedFeedPrice: (s: any) => void;
  setIsEditModalOpen: (v: boolean) => void;
  deleteFeedPrice: (id: string) => void;
}): GridColDef[] => {
  return [
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
      getActions: (params) => [
        <ActionsCell
          key="actions"
          params={params}
          onEdit={(row) => {
            setSelectedFeedPrice(row);
            setIsEditModalOpen(true);
          }}
          onDelete={deleteFeedPrice}
        />,
      ],
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];
};
