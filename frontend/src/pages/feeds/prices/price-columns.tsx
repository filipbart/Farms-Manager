import { Button } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";

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
    { field: "id", headerName: "Id", width: 70 },
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
    { field: "price", headerName: "Cena [zł]", flex: 1 },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedFeedPrice(params.row);
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
            deleteFeedPrice(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];
};
