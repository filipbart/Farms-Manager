import { Box, Tooltip } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { MdWarningAmber } from "react-icons/md";
import { orange, red } from "@mui/material/colors";
import { CommentCell } from "../../../components/datagrid/comment-cell";
import FileDownloadCell from "../../../components/datagrid/file-download-cell";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";

export const getFeedsDeliveriesColumns = ({
  setSelectedFeedDelivery,
  setIsEditModalOpen,
  setIsEditCorrectionModalOpen,
  deleteFeedDelivery,
  deleteFeedCorrection,
  downloadInvoiceFile,
  downloadCorrectionFile,
  downloadFilePath,
}: {
  setSelectedFeedDelivery: (s: any) => void;
  setIsEditModalOpen: (v: boolean) => void;
  setIsEditCorrectionModalOpen: (v: boolean) => void;
  deleteFeedDelivery: (id: string) => void;
  deleteFeedCorrection: (id: string) => void;
  downloadInvoiceFile: (path: string) => void;
  downloadCorrectionFile: (path: string) => void;
  downloadFilePath: string | null;
}): GridColDef[] => {
  return [
    { field: "cycleText", headerName: "Identyfikator", flex: 1 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "henhouseName", headerName: "Kurnik", flex: 1 },
    { field: "vendorName", headerName: "Sprzedawca", flex: 1 },
    { field: "itemName", headerName: "Nazwa paszy", flex: 1 },
    { field: "invoiceNumber", headerName: "Numer faktury", flex: 1 },
    {
      field: "quantity",
      headerName: "Ilość towaru [t]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "unitPrice",
      headerName: "Cena jednostkowa netto [zł]",
      flex: 1,
      type: "number",
      renderCell: (params) => {
        const unitPrice = params.value;
        const correctUnitPrice = params.row?.correctUnitPrice;

        const formattedPrice =
          unitPrice != null
            ? new Intl.NumberFormat("pl-PL", {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
              }).format(unitPrice)
            : "—";

        const formattedCorrectPrice =
          correctUnitPrice != null
            ? new Intl.NumberFormat("pl-PL", {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
              }).format(correctUnitPrice)
            : "";

        return (
          <Box display="flex" alignItems="center" gap={1}>
            <span>{formattedPrice}</span>
            {correctUnitPrice != null && (
              <Tooltip
                title={
                  correctUnitPrice === -1
                    ? "Sprawdź cenę"
                    : `Prawidłowa cena: ${formattedCorrectPrice} zł`
                }
              >
                <MdWarningAmber style={{ fontSize: 20, color: red[700] }} />
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
    {
      field: "invoiceTotal",
      headerName: "Wartość brutto [zł]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "subTotal",
      headerName: "Wartość netto [zł]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "vatAmount",
      headerName: "VAT [zł]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "paymentDateUtc",
      headerName: "Data opłacenia",
      flex: 1,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        const date = params.row.paymentDateUtc;
        return date
          ? `Opłacono dnia: ${new Date(date).toLocaleString()}`
          : "Nie opłacono";
      },
    },

    {
      field: "download",
      type: "actions",
      headerName: "Faktura",
      flex: 1,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        const isCorrection = params.row.isCorrection;
        const filePath = params.row.filePath;
        const id = params.row.id;

        const effectiveFilePath = isCorrection ? filePath : id;
        const onDownload = isCorrection
          ? downloadCorrectionFile
          : downloadInvoiceFile;

        return (
          <FileDownloadCell
            filePath={effectiveFilePath}
            downloadingFilePath={downloadFilePath}
            onDownload={onDownload}
          />
        );
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      minWidth: 150,
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return [
          <ActionsCell
            key="actions"
            params={params}
            onEdit={(row) => {
              setSelectedFeedDelivery(row);
              if (row.isCorrection) {
                setIsEditCorrectionModalOpen(true);
              } else {
                setIsEditModalOpen(true);
              }
            }}
            onDelete={(id) => {
              if (params.row.isCorrection) {
                deleteFeedCorrection(id);
              } else {
                deleteFeedDelivery(id);
              }
            }}
          />,
        ];
      },
    },

    {
      field: "comment",
      headerName: "Komentarz",
      flex: 1,
      aggregable: false,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return <CommentCell value={params.value} />;
      },
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];
};
