import { Button, Typography, IconButton } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { MdFileDownload } from "react-icons/md";
import type { ExpenseProductionListModel } from "../../../models/expenses/production/expenses-productions";
import Loading from "../../../components/loading/loading";

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
      field: "id",
      headerName: "Id",
      width: 70,
    },
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
      headerName: "Wartość brutto",
      type: "number",
      flex: 1,
    },
    {
      field: "subTotal",
      headerName: "Wartość netto",
      type: "number",
      flex: 1,
    },
    {
      field: "vatAmount",
      headerName: "Wartość VAT",
      type: "number",
      flex: 1,
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
      headerName: "Dokument",
      align: "center",
      headerAlign: "center",
      sortable: false,
      flex: 0.5,
      renderCell: (params) => {
        const filePath = params.row.filePath;

        if (!filePath) {
          return (
            <Typography variant="body2" color="text.secondary">
              Brak
            </Typography>
          );
        }

        const isDownloading = downloadingFilePath === filePath;

        return (
          <IconButton
            onClick={() => downloadExpenseProductionFile(filePath)}
            color="primary"
            disabled={isDownloading}
          >
            {isDownloading ? <Loading size={10} /> : <MdFileDownload />}
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
            setSelectedExpenseProduction(params.row);
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
            deleteExpenseProduction(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
  ];
};
