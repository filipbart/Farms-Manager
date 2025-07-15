import { Button } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";

export const getFeedsDeliveriesColumns = ({
  setSelectedFeedDelivery,
  setIsEditModalOpen,
  deleteFeedDelivery,
}: {
  setSelectedFeedDelivery: (s: any) => void;
  setIsEditModalOpen: (v: boolean) => void;
  deleteFeedDelivery: (id: string) => void;
}): GridColDef[] => {
  return [
    { field: "id", headerName: "Id", width: 70 },
    { field: "cycleText", headerName: "Identyfikator", flex: 1 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "henhouseName", headerName: "Kurnik", flex: 1 },
    { field: "vendor", headerName: "Sprzedawca", flex: 1 },
    { field: "feedName", headerName: "Nazwa paszy", flex: 1 },
    { field: "amount", headerName: "Ilość towaru", flex: 1 },
    {
      field: "netUnitPrice",
      headerName: "Cena jednostkowa netto [zł]",
      flex: 1,
    },
    {
      field: "invoiceDate",
      headerName: "Data wystawienia faktury",
      flex: 1,
      type: "string",
      valueGetter: (date: any) => {
        return date ? dayjs(date).format("YYYY-MM-DD") : "";
      },
    },
    {
      field: "dueDate",
      headerName: "Termin płatności",
      flex: 1,
      type: "string",
      valueGetter: (date: any) => {
        return date ? dayjs(date).format("YYYY-MM-DD") : "";
      },
    },
    { field: "gross", headerName: "Wartość brutto [zł]", flex: 1 },
    { field: "netWorth", headerName: "Wartość netto [zł]", flex: 1 },
    { field: "vat", headerName: "VAT [zł]", flex: 1 },
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
            setSelectedFeedDelivery(params.row);
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
            deleteFeedDelivery(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];
};
