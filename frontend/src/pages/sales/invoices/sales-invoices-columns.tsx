import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { getAuditColumns } from "../../../utils/audit-columns-helper";
import type { SalesInvoiceListModel } from "../../../models/sales/sales-invoices";
import { SalesInvoiceStatus } from "../../../models/sales/sales-invoices";
import FileDownloadCell from "../../../components/datagrid/file-download-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { Button, Chip } from "@mui/material";
import { CommentCell } from "../../../components/datagrid/comment-cell";

interface GetSalesInvoicesColumnsProps {
  setSelectedSalesInvoice: (row: SalesInvoiceListModel) => void;
  deleteSalesInvoice: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  downloadSalesInvoiceFile: (path: string) => void;
  downloadingFilePath: string | null;
  isAdmin?: boolean;
  onMarkAsCompleted?: (id: string) => void;
}

export const getSalesInvoicesColumns = ({
  setSelectedSalesInvoice,
  deleteSalesInvoice,
  setIsEditModalOpen,
  downloadSalesInvoiceFile,
  downloadingFilePath,
  isAdmin = false,
  onMarkAsCompleted,
}: GetSalesInvoicesColumnsProps): GridColDef<SalesInvoiceListModel>[] => {
  const baseColumns: GridColDef[] = [
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
      field: "paymentDate",
      headerName: "Data płatności",
      width: 150,
      valueGetter: (value: string | null) =>
        value ? dayjs(value).format("YYYY-MM-DD") : "—",
    },
    {
      field: "invoiceTotal",
      headerName: "Brutto [zł]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "subTotal",
      headerName: "Netto [zł]",
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
      field: "status",
      headerName: "Status",
      flex: 1,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return null;
        }
        const isCompleted = params.row.status === SalesInvoiceStatus.Realized;
        return (
          <Chip
            label={params.row.status || SalesInvoiceStatus.Unrealized}
            color={isCompleted ? "success" : "error"}
            size="small"
          />
        );
      },
    },
    {
      field: "comment",
      headerName: "Komentarz",
      flex: 1.5,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return null;
        }
        return params.row.comment ? (
          <CommentCell value={params.row.comment} />
        ) : (
          <span>-</span>
        );
      },
    },
    {
      field: "filePath",
      headerName: "Faktura",
      align: "center",
      headerAlign: "center",
      sortable: false,
      width: 100,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return (
          <FileDownloadCell
            filePath={params.row.filePath}
            downloadingFilePath={downloadingFilePath}
            onDownload={downloadSalesInvoiceFile}
          />
        );
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 250,
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        const isCompleted = params.row.status === SalesInvoiceStatus.Realized;
        const actions = [];

        if (!isCompleted && onMarkAsCompleted) {
          actions.push(
            <Button
              key="complete"
              size="small"
              variant="outlined"
              color="success"
              onClick={() => onMarkAsCompleted(params.row.id)}
            >
              Zrealizowany
            </Button>
          );
        }

        actions.push(
          <ActionsCell
            key="actions"
            params={params}
            onEdit={(row) => {
              setSelectedSalesInvoice(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteSalesInvoice}
          />
        );

        return actions;
      },
    },
  ];

  const auditColumns = getAuditColumns<SalesInvoiceListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
