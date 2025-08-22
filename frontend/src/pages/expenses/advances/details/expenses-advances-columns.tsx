import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../../../components/datagrid/actions-cell";
import { CommentCell } from "../../../../components/datagrid/comment-cell";
import FileDownloadCell from "../../../../components/datagrid/file-download-cell";
import type { ExpenseAdvanceListModel } from "../../../../models/expenses/advances/expenses-advances";
import { AdvanceType } from "../../../../models/expenses/advances/categories";

interface GetAdvancesColumnsProps {
  setSelectedAdvance: (row: ExpenseAdvanceListModel) => void;
  deleteAdvance: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  downloadAdvanceFile: (path: string) => void;
  downloadingFilePath: string | null;
}

export const getAdvancesColumns = ({
  setSelectedAdvance,
  deleteAdvance,
  setIsEditModalOpen,
  downloadAdvanceFile,
  downloadingFilePath,
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
      renderCell: (params) => (
        <FileDownloadCell
          filePath={params.row.filePath}
          downloadingFilePath={downloadingFilePath}
          onDownload={downloadAdvanceFile}
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
            setSelectedAdvance(row);
            setIsEditModalOpen(true);
          }}
          onDelete={deleteAdvance}
        />,
      ],
    },
  ];
};
