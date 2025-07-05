import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
} from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { toast } from "react-toastify";
import { SalesService } from "../../services/sales-service";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import Loading from "../../components/loading/loading";
import { OtherExtrasCell } from "../../models/sales/sale-other-extras-cell";
import { useState } from "react";

const SaleCommentCell: React.FC<{ value: string }> = ({ value }) => {
  const [open, setOpen] = useState(false);

  if (!value) return null;

  const preview = value.length > 20 ? `${value.slice(0, 20)}...` : value;

  return (
    <>
      <Button variant="text" onClick={() => setOpen(true)}>
        {preview}
      </Button>
      <Dialog open={open} onClose={() => setOpen(false)}>
        <DialogTitle>Zawartość komentarza</DialogTitle>
        <DialogContent>
          <Typography>{value}</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Zamknij</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export const getSalesColumns = ({
  setSelectedSale,
  setIsEditModalOpen,
  dispatch,
  filters,
}: {
  setSelectedSale: (s: any) => void;
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
      field: "saleDate",
      headerName: "Data sprzedaży",
      flex: 1,
      type: "string",
      valueGetter: (params: any) => dayjs(params.value).format("YYYY-MM-DD"),
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
      renderCell: (params) => <SaleCommentCell value={params.value} />,
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
      ],
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
