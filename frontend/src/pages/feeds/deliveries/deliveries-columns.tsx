import { Box, Button, IconButton } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { MdFileDownload } from "react-icons/md";
import Loading from "../../../components/loading/loading";

export const getFeedsDeliveriesColumns = ({
  setSelectedFeedDelivery,
  setIsEditModalOpen,
  deleteFeedDelivery,
  downloadInvoiceFile,
  loadingFileId,
}: {
  setSelectedFeedDelivery: (s: any) => void;
  setIsEditModalOpen: (v: boolean) => void;
  deleteFeedDelivery: (id: string) => void;
  downloadInvoiceFile: (id: string) => void;
  loadingFileId: string | null;
}): GridColDef[] => {
  return [
    { field: "id", headerName: "Id", width: 70 },
    { field: "cycleText", headerName: "Identyfikator", flex: 1 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "henhouseName", headerName: "Kurnik", flex: 1 },
    { field: "vendorName", headerName: "Sprzedawca", flex: 1 },
    { field: "itemName", headerName: "Nazwa paszy", flex: 1 },
    { field: "quantity", headerName: "Ilość towaru", flex: 1 },
    {
      field: "unitPrice",
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
    { field: "invoiceTotal", headerName: "Wartość brutto [zł]", flex: 1 },
    { field: "subTotal", headerName: "Wartość netto [zł]", flex: 1 },
    { field: "vatAmount", headerName: "VAT [zł]", flex: 1 },
    {
      field: "invoiceFile",
      headerName: "Plik faktury",
      flex: 1,
      renderCell: (params) => (
        <Box
          display="flex"
          justifyContent="center"
          alignItems="center"
          width="100%"
          height="100%"
        >
          <IconButton
            onClick={() => downloadInvoiceFile(params.row.id)}
            color="primary"
            disabled={loadingFileId === params.row.id}
          >
            {loadingFileId === params.row.id ? (
              <Loading size={10} />
            ) : (
              <MdFileDownload />
            )}
          </IconButton>
        </Box>
      ),
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      minWidth: 150,
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
    { field: "comment", headerName: "Notatka", flex: 1 },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];
};
