import { Box, Button, IconButton, Typography } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { MdFileDownload } from "react-icons/md";
import Loading from "../../../components/loading/loading";
import type { SalesInvoiceListModel } from "../../../models/sales/sales-invoices";

interface GetSalesInvoicesColumnsProps {
  setSelectedSalesInvoice: (row: SalesInvoiceListModel) => void;
  deleteSalesInvoice: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  downloadSalesInvoiceFile: (path: string) => void;
  downloadingFilePath: string | null;
}

export const getSalesInvoicesColumns = ({
  setSelectedSalesInvoice,
  deleteSalesInvoice,
  setIsEditModalOpen,
  downloadSalesInvoiceFile,
  downloadingFilePath,
}: GetSalesInvoicesColumnsProps): GridColDef<SalesInvoiceListModel>[] => {
  return [
    {
      field: "cycleText",
      headerName: "Cykl",
      flex: 1,
    },
    {
      field: "farmName",
      headerName: "Ferma",
      flex: 1.5,
    },
    {
      field: "slaughterhouseName",
      headerName: "Nabywca",
      flex: 1.5,
    },
    {
      field: "invoiceNumber",
      headerName: "Numer faktury",
      flex: 1,
    },
    {
      field: "invoiceDate",
      headerName: "Data wystawienia",
      width: 150,
      valueGetter: (value: string) =>
        value ? dayjs(value).format("YYYY-MM-DD") : "",
    },
    {
      field: "dueDate",
      headerName: "Termin płatności",
      width: 150,
      valueGetter: (value: string) =>
        value ? dayjs(value).format("YYYY-MM-DD") : "",
    },
    {
      field: "invoiceTotal",
      headerName: "Brutto [zł]",
      flex: 1,
    },
    {
      field: "subTotal",
      headerName: "Netto [zł]",
      flex: 1,
    },
    {
      field: "vatAmount",
      headerName: "VAT [zł]",
      flex: 1,
    },
    {
      field: "filePath",
      headerName: "Faktura",
      align: "center",
      headerAlign: "center",
      sortable: false,
      width: 100,
      renderCell: (params) => {
        const filePath = params.row.filePath;
        if (!filePath) {
          return (
            <Box
              sx={{
                width: "100%",
                height: "100%",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              <Typography variant="body2" color="text.secondary">
                Brak
              </Typography>
            </Box>
          );
        }
        const isDownloading = downloadingFilePath === filePath;
        return (
          <IconButton
            onClick={() => downloadSalesInvoiceFile(filePath)}
            color="primary"
            disabled={isDownloading}
          >
            {isDownloading ? <Loading size={24} /> : <MdFileDownload />}
          </IconButton>
        );
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 200,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedSalesInvoice(params.row);
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
            deleteSalesInvoice(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
  ];
};
