import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import FileDownloadCell from "../../../components/datagrid/file-download-cell";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { Button, Chip } from "@mui/material";
import { CommentCell } from "../../../components/datagrid/comment-cell";
import { getAuditColumns } from "../../../utils/audit-columns-helper";
import {
  type FeedPaymentListModel,
  FeedPaymentStatus,
  FeedPaymentStatusLabels,
} from "../../../models/feeds/payments/payment";

export const getFeedsPaymentsColumns = ({
  deleteFeedPayment,
  downloadPaymentFile,
  downloadFilePath,
  onMarkAsCompleted,
  isAdmin = false,
}: {
  deleteFeedPayment: (id: string) => void;
  downloadPaymentFile: (path: string) => void;
  downloadFilePath: string | null;
  onMarkAsCompleted: (id: string) => void;
  isAdmin?: boolean;
}): GridColDef[] => {
  const baseColumns: GridColDef[] = [
    {
      field: "cycleText",
      headerName: "Identyfikator",
      flex: 1,
    },
    { field: "farmName", headerName: "Nazwa farmy", flex: 1 },
    { field: "fileName", headerName: "Nazwa pliku", flex: 1 },
    {
      field: "status",
      headerName: "Status",
      flex: 1,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return null;
        }
        const isCompleted = params.row.status === FeedPaymentStatus.Realized;
        return (
          <Chip
            label={
              FeedPaymentStatusLabels[params.row.status as FeedPaymentStatus]
            }
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
      align: "center",
      flex: 1,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return (
          <FileDownloadCell
            filePath={params.row.filePath}
            downloadingFilePath={downloadFilePath}
            onDownload={downloadPaymentFile}
          />
        );
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        const isCompleted = params.row.status === FeedPaymentStatus.Realized;
        const actions = [];

        if (!isCompleted) {
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
            onDelete={deleteFeedPayment}
          />
        );

        return actions;
      },
    },
  ];

  const auditColumns = getAuditColumns<FeedPaymentListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
