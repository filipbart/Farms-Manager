import { Box, Button, IconButton, Typography } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { toast } from "react-toastify";
import { SalesService } from "../../services/sales-service";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import Loading from "../../components/loading/loading";
import { OtherExtrasCell } from "../../models/sales/sale-other-extras-cell";
import { useState } from "react";
import { MdFileDownload } from "react-icons/md";
import { CommentCell } from "../../components/datagrid/comment-cell";

export const getSalesColumns = ({
  setSelectedSale,
  deleteSale,
  setIsEditModalOpen,
  downloadSaleDirectory,
  downloadDirectoryPath,
  dispatch,
  filters,
}: {
  setSelectedSale: (s: any) => void;
  deleteSale: (id: string) => void;
  setIsEditModalOpen: (v: boolean) => void;
  downloadSaleDirectory: (path: string) => void;
  downloadDirectoryPath: string | null;
  dispatch: any;
  filters: any;
}): GridColDef[] => {
  return [
    { field: "id", headerName: "Id", width: 70 },
    { field: "cycleText", headerName: "Identyfikator", flex: 1 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "henhouseName", headerName: "Kurnik", flex: 1 },
    { field: "slaughterhouseName", headerName: "Ubojnia", flex: 1 },
    {
      field: "saleDate",
      headerName: "Data sprzedaży",
      flex: 1,
      type: "string",
      valueGetter: (date: any) => {
        return date ? dayjs(date).format("YYYY-MM-DD") : "";
      },
    },
    { field: "typeDesc", headerName: "Typ sprzedaży", flex: 1 },
    { field: "weight", headerName: "Waga ubojni [kg]", flex: 1 },
    { field: "quantity", headerName: "Ilość sztuk ubojnia [szt]", flex: 1 },
    { field: "confiscatedWeight", headerName: "Konfiskaty [kg]", flex: 1 },
    { field: "confiscatedCount", headerName: "Konfiskaty [szt]", flex: 1 },
    { field: "deadWeight", headerName: "Kurczęta martwe [kg]", flex: 1 },
    { field: "deadCount", headerName: "Kurczęta martwe [szt]", flex: 1 },
    { field: "farmerWeight", headerName: "Waga producenta [kg]", flex: 1 },
    { field: "basePrice", headerName: "Cena bazowa [zł]", flex: 1 },
    { field: "priceWithExtras", headerName: "Cena z dodatkami [zł]", flex: 1 },
    {
      field: "otherExtras",
      headerName: "Inne dodatki",
      flex: 1,
      renderCell: (params) => <OtherExtrasCell value={params.value} />,
    },
    {
      field: "comment",
      headerName: "Komentarz",
      flex: 1,
      renderCell: (params) => <CommentCell value={params.value} />,
    },
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
          saleId?: string;
        }) => {
          setLoadingSendToIrz(true);
          await handleApiResponse(
            () => SalesService.sendToIrzPlus(data),
            async () => {
              toast.success("Wysłano do IRZplus");
              dispatch({
                type: "setMultiple",
                payload: { page: filters.page },
              });
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
                  onClick={() => handleSendToIrz({ saleId: params.row.id })}
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
      minWidth: 200,
      flex: 1,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedSale(params.row);
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
            deleteSale(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
    {
      field: "fileDownload",
      headerName: "Zawarte dokumenty",
      flex: 1,
      renderCell: (params) => {
        const directoryPath = params.row.directoryPath;

        if (!directoryPath) {
          return (
            <Box
              width="100%"
              height="100%"
              display="flex"
              justifyContent="center"
              alignItems="center"
            >
              <Typography variant="body2" color="text.secondary">
                Brak
              </Typography>
            </Box>
          );
        }

        return (
          <Box
            display="flex"
            justifyContent="center"
            alignItems="center"
            width="100%"
            height="100%"
          >
            <IconButton
              onClick={() => downloadSaleDirectory(directoryPath)}
              color="primary"
              disabled={downloadDirectoryPath === directoryPath}
            >
              {downloadDirectoryPath === directoryPath ? (
                <Loading size={10} />
              ) : (
                <MdFileDownload />
              )}
            </IconButton>
          </Box>
        );
      },
    },
    {
      field: "documentNumber",
      headerName: "Numer dokumentu IRZplus",
      flex: 1,
      renderCell: (params) => (params.value ? params.value : "Brak numeru"),
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      flex: 1,
    },
  ];
};
