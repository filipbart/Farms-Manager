import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import FileDownloadCell from "../../../components/datagrid/file-download-cell";
import ActionsCell from "../../../components/datagrid/actions-cell";

export const getFeedsPaymentsColumns = ({
  deleteFeedPayment,
  downloadPaymentFile,
  downloadFilePath,
}: {
  deleteFeedPayment: (id: string) => void;
  downloadPaymentFile: (path: string) => void;
  downloadFilePath: string | null;
}): GridColDef[] => {
  return [
    { field: "farmName", headerName: "Nazwa farmy", flex: 1 },
    { field: "fileName", headerName: "Nazwa pliku", flex: 1 },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      flex: 1,
      type: "string",
      valueGetter: (date: any) => {
        return date ? dayjs(date).format("YYYY-MM-DD, HH:mm:ss") : "";
      },
    },
    {
      field: "fileDownload",
      headerName: "Faktura",
      flex: 1,
      renderCell: (params) => (
        <FileDownloadCell
          filePath={params.row.filePath}
          downloadingFilePath={downloadFilePath}
          onDownload={downloadPaymentFile}
        />
      ),
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      getActions: (params) => [
        <ActionsCell
          key="actions"
          params={params}
          onDelete={deleteFeedPayment}
        />,
      ],
    },
  ];
};
