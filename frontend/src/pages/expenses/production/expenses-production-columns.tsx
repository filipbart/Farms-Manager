import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { ExpenseProductionListModel } from "../../../models/expenses/production/expenses-productions";
import ActionsCell from "../../../components/datagrid/actions-cell";
import FileDownloadCell from "../../../components/datagrid/file-download-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { CommentCell } from "../../../components/datagrid/comment-cell";

interface GetExpenseProductionColumnsProps {
  setSelectedExpenseProduction: (row: ExpenseProductionListModel) => void;
  deleteExpenseProduction: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  downloadExpenseProductionFile: (path: string) => void;
  downloadingFilePath: string | null;
}

export const getExpenseProductionColumns = ({
  setSelectedExpenseProduction,
  deleteExpenseProduction,
  setIsEditModalOpen,
  downloadExpenseProductionFile,
  downloadingFilePath,
}: GetExpenseProductionColumnsProps): GridColDef<ExpenseProductionListModel>[] => {
  return [
    {
      field: "cycleText",
      headerName: "Cykl",
      flex: 1,
    },
    {
      field: "farmName",
      headerName: "Ferma",
      flex: 1,
    },
    {
      field: "contractorName",
      headerName: "Sprzedawca",
      flex: 1,
    },
    {
      field: "invoiceNumber",
      headerName: "Numer faktury",
      flex: 1,
    },
    {
      field: "expenseTypeName",
      headerName: "Typ wydatku",
      flex: 1,
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
      headerName: "Wartość VAT [zł]",

      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "invoiceDate",
      headerName: "Data wystawienia faktury",
      flex: 1,
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD") : "";
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
    {
      field: "filePath",
      headerName: "Faktura",
      align: "center",
      headerAlign: "center",
      sortable: false,
      flex: 0.5,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return (
          <FileDownloadCell
            filePath={params.row.filePath}
            downloadingFilePath={downloadingFilePath}
            onDownload={downloadExpenseProductionFile}
          />
        );
      },
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
              setSelectedExpenseProduction(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteExpenseProduction}
          />,
        ];
      },
    },
    {
      field: "comment",
      headerName: "Komentarz",
      align: "center",
      headerAlign: "center",
      flex: 1,
      sortable: false,
      aggregable: false,
      renderCell: (params) => <CommentCell value={params.value} />,
    },
  ];
};
