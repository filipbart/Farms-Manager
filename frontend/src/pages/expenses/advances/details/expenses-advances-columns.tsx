import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../../../components/datagrid/actions-cell";
import { getAuditColumns } from "../../../../utils/audit-columns-helper";
import type { ExpenseAdvanceListModel } from "../../../../models/expenses/advances/expenses-advances";
import { CommentCell } from "../../../../components/datagrid/comment-cell";
import FileDownloadCell from "../../../../components/datagrid/file-download-cell";
import { AdvanceType } from "../../../../models/expenses/advances/categories";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";

interface GetAdvancesColumnsProps {
  setSelectedAdvance: (row: ExpenseAdvanceListModel) => void;
  deleteAdvance: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  downloadAdvanceFile: (path: string) => void;
  downloadingFilePath: string | null;
  isAdmin?: boolean;
  hasEditPermission: (employeeId: string) => boolean;
  employeeId: string;
}

export const getAdvancesColumns = ({
  setSelectedAdvance,
  deleteAdvance,
  setIsEditModalOpen,
  downloadAdvanceFile,
  downloadingFilePath,
  isAdmin = false,
  hasEditPermission,
  employeeId,
}: GetAdvancesColumnsProps): GridColDef<ExpenseAdvanceListModel>[] => {
  const baseColumns: GridColDef<ExpenseAdvanceListModel>[] = [
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
      field: "comment",
      headerName: "Komentarz",
      flex: 1,
      sortable: false,
      aggregable: false,
      renderCell: (params) => <CommentCell value={params.value} />,
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
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      flex: 1,
      valueGetter: (value: string) =>
        value ? dayjs(value).format("YYYY-MM-DD HH:mm:ss") : "",
    },
  ];

  // Only add actions column if user has admin permissions or edit permissions for this employee
  const canEdit = isAdmin || hasEditPermission(employeeId);
  if (canEdit) {
    baseColumns.push({
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
              setSelectedAdvance(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteAdvance}
          />,
        ];
      },
    });
  }

  const auditColumns = getAuditColumns<ExpenseAdvanceListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
