import { Box, Button, Typography } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import Loading from "../../components/loading/loading";
import { useState } from "react";
import { InsertionsService } from "../../services/insertions-service";

export const getInsertionsColumns = ({
  setSelectedInsertion,
  setIsEditModalOpen,
  dispatch,
  filters,
}: {
  setSelectedInsertion: (s: any) => void;
  setIsEditModalOpen: (v: boolean) => void;
  dispatch: any;
  filters: any;
}): GridColDef[] => {
  return [
    { field: "id", headerName: "Id", width: 70 },
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
    { field: "quantity", headerName: "Sztuki wstawione", flex: 1 },
    { field: "hatcheryName", headerName: "Wylęgarnia", flex: 1 },
    { field: "bodyWeight", headerName: "Śr. masa ciała", flex: 1 },
    {
      field: "sendToIrz",
      headerName: "Wyślij do IRZplus",
      flex: 1,

      minWidth: 200,
      type: "actions",
      renderCell: (params) => {
        const { isSentToIrz, dateIrzSentUtc } = params.row;
        const [loadingSendToIrz, setLoadingSendToIrz] = useState(false);
        const handleSendToIrz = async (data: {
          internalGroupId?: string;
          insertionId?: string;
        }) => {
          setLoadingSendToIrz(true);
          await handleApiResponse(
            () => InsertionsService.sendToIrzPlus(data),
            async () => {
              toast.success("Wysłano do IRZplus");
              dispatch({
                type: "setMultiple",
                payload: { page: filters.page },
              });
              setLoadingSendToIrz(false);
            },
            undefined,
            "Wystąpił błąd podczas wysyłania do IRZplus"
          );
          setLoadingSendToIrz(false);
        };
        if (dateIrzSentUtc) {
          const formattedDate = new Date(dateIrzSentUtc).toLocaleString(
            "pl-PL",
            {
              dateStyle: "short",
              timeStyle: "short",
            }
          );

          return (
            <Typography variant="body2" sx={{ whiteSpace: "nowrap" }}>
              Wysłano - {formattedDate}
            </Typography>
          );
        }

        if (isSentToIrz) {
          return null;
        }

        return (
          <Box
            sx={{
              display: "flex",
              flexDirection: "row",
              alignItems: "center",
              gap: 1,
              flexWrap: "nowrap",
            }}
          >
            {loadingSendToIrz ? (
              <Loading height="0" size={10} />
            ) : (
              <>
                <Button
                  variant="contained"
                  color="error"
                  size="small"
                  onClick={() =>
                    handleSendToIrz({ insertionId: params.row.id })
                  }
                >
                  Osobno
                </Button>

                <Button
                  variant="outlined"
                  color="error"
                  size="small"
                  onClick={() =>
                    handleSendToIrz({
                      internalGroupId: params.row.internalGroupId,
                    })
                  }
                >
                  Z grupą
                </Button>
              </>
            )}
          </Box>
        );
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedInsertion(params.row);
            setIsEditModalOpen(true);
          }}
        >
          Edytuj
        </Button>,
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
