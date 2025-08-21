import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import ActionsCell from "../../components/datagrid/actions-cell";
import InsertionSendToIrzCell from "../../components/datagrid/insertion-send-to-irz-cell";
import { Box, Tooltip } from "@mui/material";
import { FaCheckCircle, FaTimesCircle } from "react-icons/fa";

export const getInsertionsColumns = ({
  setSelectedInsertion,
  deleteInsertion,
  setIsEditModalOpen,
  dispatch,
  filters,
}: {
  setSelectedInsertion: (s: any) => void;
  deleteInsertion: (id: string) => void;
  setIsEditModalOpen: (v: boolean) => void;
  dispatch: any;
  filters: any;
}): GridColDef[] => {
  return [
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
      field: "reportedToWios",
      headerName: "Status WIOŚ",
      width: 120,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => {
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
      renderCell: (params) => (
        <InsertionSendToIrzCell
          isSentToIrz={params.row.isSentToIrz}
          dateIrzSentUtc={params.row.dateIrzSentUtc}
          insertionId={params.row.id}
          internalGroupId={params.row.internalGroupId}
          dispatch={dispatch}
          filters={filters}
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
            setSelectedInsertion(row);
            setIsEditModalOpen(true);
          }}
          onDelete={deleteInsertion}
        />,
      ],
    },

    {
      field: "documentNumber",
      headerName: "Numer dokumentu IRZplus",
      flex: 1,
      renderCell: (params) => {
        return params.value ? params.value : "Brak numeru";
      },
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];
};
