import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { SalesInvoiceListModel } from "../../../models/sales/sales-invoices";
import ActionsCell from "../../../components/datagrid/actions-cell";
import FileDownloadCell from "../../../components/datagrid/file-download-cell";

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
      field: "filePath",
      headerName: "Faktura",
      align: "center",
      headerAlign: "center",
      sortable: false,
      width: 100,
      renderCell: (params) => (
        <FileDownloadCell
          filePath={params.row.filePath}
          downloadingFilePath={downloadingFilePath}
          onDownload={downloadSalesInvoiceFile}
        />
      ),
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
            setSelectedSalesInvoice(row);
            setIsEditModalOpen(true);
          }}
          onDelete={deleteSalesInvoice}
        />,
      ],
    },
  ];
};
