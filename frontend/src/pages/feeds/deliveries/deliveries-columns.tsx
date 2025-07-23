import { Box, Button, IconButton, Tooltip } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { MdFileDownload, MdWarningAmber } from "react-icons/md";
import Loading from "../../../components/loading/loading";
import { orange, red } from "@mui/material/colors";

export const getFeedsDeliveriesColumns = ({
  setSelectedFeedDelivery,
  setIsEditModalOpen,
  setIsEditCorrectionModalOpen,
  deleteFeedDelivery,
  downloadInvoiceFile,
  downloadCorrectionFile,
  downloadFilePath,
}: {
  setSelectedFeedDelivery: (s: any) => void;
  setIsEditModalOpen: (v: boolean) => void;
  setIsEditCorrectionModalOpen: (v: boolean) => void;
  deleteFeedDelivery: (id: string) => void;
  downloadInvoiceFile: (path: string) => void;
  downloadCorrectionFile: (path: string) => void;
  downloadFilePath: string | null;
}): GridColDef[] => {
  return [
    { field: "id", headerName: "Id", width: 70 },
    { field: "cycleText", headerName: "Identyfikator", flex: 1 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "henhouseName", headerName: "Kurnik", flex: 1 },
    { field: "vendorName", headerName: "Sprzedawca", flex: 1 },
    { field: "itemName", headerName: "Nazwa paszy", flex: 1 },
    { field: "invoiceNumber", headerName: "Numer faktury", flex: 1 },
    {
      field: "quantity",
      headerName: "Ilość towaru",
      flex: 1,
      renderCell: (params) => {
        const value = params.value;
        return <span>{value != null ? value : "—"}</span>;
      },
    },
    {
      field: "unitPrice",
      headerName: "Cena jednostkowa netto [zł]",
      flex: 1,
      renderCell: (params) => {
        const unitPrice = params.value;
        const correctUnitPrice = params.row?.correctUnitPrice;

        return (
          <Box display="flex" alignItems="center">
            <span>{unitPrice != null ? unitPrice : "—"}</span>
            {correctUnitPrice != null && (
              <Tooltip title={`Prawidłowa cena: ${correctUnitPrice} zł`}>
                <MdWarningAmber
                  style={{ marginLeft: 8, fontSize: 20, color: red[900] }}
                />
              </Tooltip>
            )}
          </Box>
        );
      },
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
      renderCell: (params) => {
        const dueDate = params.row?.dueDate;
        const paymentDateUtc = params.row?.paymentDateUtc;
        const isCorrection = params.row?.isCorrection;

        const isOverdue =
          !isCorrection &&
          !paymentDateUtc &&
          dueDate &&
          dayjs(dueDate).isBefore(dayjs(), "day");

        return (
          <Box display="flex" alignItems="center">
            {isOverdue && (
              <Tooltip title="Termin płatności minął">
                <MdWarningAmber
                  style={{ marginRight: 8, fontSize: 20, color: orange[700] }}
                />
              </Tooltip>
            )}
            <span>{dueDate ? dayjs(dueDate).format("YYYY-MM-DD") : "—"}</span>
          </Box>
        );
      },
    },
    { field: "invoiceTotal", headerName: "Wartość brutto [zł]", flex: 1 },
    { field: "subTotal", headerName: "Wartość netto [zł]", flex: 1 },
    { field: "vatAmount", headerName: "VAT [zł]", flex: 1 },
    {
      field: "paymentDateUtc",
      headerName: "Data opłacenia",
      flex: 1,
      renderCell: (params) => {
        const date = params.row.paymentDateUtc;
        return date
          ? `Opłacono dnia: ${new Date(date).toLocaleString()}`
          : "Nie opłacono";
      },
    },

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
            onClick={() =>
              params.row.isCorrection
                ? downloadCorrectionFile(params.row.filePath)
                : downloadInvoiceFile(params.row.id)
            }
            color="primary"
            disabled={
              params.row.isCorrection
                ? downloadFilePath === params.row.filePath
                : downloadFilePath === params.row.id
            }
          >
            {(
              params.row.isCorrection
                ? downloadFilePath === params.row.filePath
                : downloadFilePath === params.row.id
            ) ? (
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
            if (params.row.isCorrection) {
              setIsEditCorrectionModalOpen(true);
            } else {
              setIsEditModalOpen(true);
            }
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
