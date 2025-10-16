import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../../../components/datagrid/actions-cell";
import { CommentCell } from "../../../../components/datagrid/comment-cell";
import FileDownloadCell from "../../../../components/datagrid/file-download-cell";
import type { ExpenseAdvanceListModel } from "../../../../models/expenses/advances/expenses-advances";
import { AdvanceType } from "../../../../models/expenses/advances/categories";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { Button, Chip } from "@mui/material";

interface GetAdvancesColumnsProps {
  setSelectedAdvance: (row: ExpenseAdvanceListModel) => void;
  deleteAdvance: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  downloadAdvanceFile: (path: string) => void;
  downloadingFilePath: string | null;
  onMarkAsCompleted: (id: string) => void;
  onMarkAsUnrealized: (id: string) => void;
}

export const getAdvancesColumns = ({
  setSelectedAdvance,
  deleteAdvance,
  setIsEditModalOpen,
  downloadAdvanceFile,
  downloadingFilePath,
  onMarkAsCompleted,
  onMarkAsUnrealized,
}: GetAdvancesColumnsProps): GridColDef<ExpenseAdvanceListModel>[] => {
  return [
    {
      field: "date",
      headerName: "Data",
      width: 150,
      valueGetter: (value: string) =>
        value ? dayjs(value).format("YYYY-MM-DD") : "",
    },
    {
      field: "type",
      headerName: "Typ",
      width: 120,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        const type = params.value as AdvanceType;
        return type === AdvanceType.Income ? "Przychód" : "Wydatek";
      },
    },
    {
      field: "name",
      headerName: "Nazwa",
      flex: 1.5,
    },
    {
      field: "amount",
      headerName: "Kwota [zł]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "categoryName",
      headerName: "Kategoria",
      flex: 1,
    },
    {
      field: "status",
      headerName: "Status",
      width: 150,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return null;
        }
        const isCompleted = params.row.status === "Zrealizowany";
        return (
          <Chip
            label={params.row.status}
            color={isCompleted ? "success" : "error"}
            size="small"
          />
        );
      },
    },
    {
      field: "paymentDate",
      headerName: "Data płatności",
      width: 150,
      valueGetter: (value: string) =>
        value ? dayjs(value).format("YYYY-MM-DD") : "-",
    },
    {
      field: "comment",
      headerName: "Komentarz",
      flex: 1,
      sortable: false,
      aggregable: false,
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
      headerName: "Plik",
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
            onDownload={downloadAdvanceFile}
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
        const isCompleted = params.row.status === "Zrealizowany";
        const actions = [];

        if (isCompleted) {
          actions.push(
            <Button
              key="unrealized"
              size="small"
              variant="outlined"
              color="error"
              onClick={() => onMarkAsUnrealized(params.row.id)}
            >
              Niezrealizowany
            </Button>
          );
        } else {
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
              setSelectedAdvance(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteAdvance}
          />
        );

        return actions;
      },
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      width: 180,
      valueGetter: (value: string) =>
        value ? dayjs(value).format("YYYY-MM-DD HH:mm") : "",
    },
  ];
};
