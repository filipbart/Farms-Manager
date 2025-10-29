import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../components/datagrid/actions-cell";
import InsertionSendToIrzCell from "../../components/datagrid/insertion-send-to-irz-cell";
import { Box, Tooltip } from "@mui/material";
import { FaCheckCircle, FaTimesCircle } from "react-icons/fa";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import { getAuditColumns } from "../../utils/audit-columns-helper";
import type InsertionListModel from "../../models/insertions/insertions";
import { CommentCell } from "../../components/datagrid/comment-cell";

export const getInsertionsColumns = ({
  setSelectedInsertion,
  deleteInsertion,
  setIsEditModalOpen,
  dispatch,
  filters,
  isAdmin = false,
}: {
  setSelectedInsertion: (s: any) => void;
  deleteInsertion: (id: string) => void;
  setIsEditModalOpen: (v: boolean) => void;
  dispatch: any;
  filters: any;
  isAdmin?: boolean;
}): GridColDef[] => {
  const baseColumns: GridColDef[] = [
    { field: "cycleText", headerName: "Identyfikator", flex: 1 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "henhouseName", headerName: "Kurnik", flex: 1 },
    {
      field: "insertionDate",
      headerName: "Data wstawienia",
      flex: 1,
      type: "string",
      valueGetter: (date: any) => {
        return date ? dayjs(date).format("YYYY-MM-DD") : "";
      },
    },
    {
      field: "quantity",
      headerName: "Sztuki wstawione",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    { field: "hatcheryName", headerName: "Wylęgarnia", flex: 1 },
    {
      field: "bodyWeight",
      headerName: "Śr. masa ciała",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
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
      field: "reportedToWios",
      headerName: "Status WIOŚ",
      width: 120,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        const isReported = params.value === true;
        const comment = params.row.wiosComment;

        const icon = isReported ? (
          <FaCheckCircle color="green" size={20} />
        ) : (
          <FaTimesCircle color="red" size={20} />
        );

        const content = comment ? (
          <Tooltip title={comment}>{icon}</Tooltip>
        ) : (
          icon
        );

        return (
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              width: "100%",
              height: "100%",
            }}
          >
            {content}
          </Box>
        );
      },
    },
    {
      field: "sendToIrz",
      headerName: "Wyślij do IRZplus",
      headerAlign: "center",
      align: "center",
      flex: 1,
      minWidth: 200,
      sortable: false,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return (
          <InsertionSendToIrzCell
            isSentToIrz={params.row.isSentToIrz}
            dateIrzSentUtc={params.row.dateIrzSentUtc}
            insertionId={params.row.id}
            internalGroupId={params.row.internalGroupId}
            irzComment={params.row.irzComment}
            dispatch={dispatch}
            filters={filters}
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
              setSelectedInsertion(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteInsertion}
          />,
        ];
      },
    },

    {
      field: "documentNumber",
      headerName: "Numer dokumentu IRZplus",
      flex: 1,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return params.value ? params.value : "Brak numeru";
      },
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];

  const auditColumns = getAuditColumns<InsertionListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
