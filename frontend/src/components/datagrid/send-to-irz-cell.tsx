import { Box, Button, Typography } from "@mui/material";
import { useState } from "react";
import { toast } from "react-toastify";
import { SalesService } from "../../services/sales-service";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import Loading from "../loading/loading";

interface SendToIrzCellProps {
  dispatch: any;
  row: {
    id: string;
    internalGroupId?: string;
    isSentToIrz?: boolean;
    dateIrzSentUtc?: string;
  };
  filters: {
    page: number;
  };
}

const SendToIrzCell = ({ dispatch, row, filters }: SendToIrzCellProps) => {
  const { isSentToIrz, dateIrzSentUtc, internalGroupId, id } = row;
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
    const formattedDate = new Date(dateIrzSentUtc).toLocaleString("pl-PL", {
      dateStyle: "short",
      timeStyle: "short",
    });

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
            onClick={() => handleSendToIrz({ saleId: id })}
          >
            Osobno
          </Button>

          <Button
            variant="outlined"
            color="error"
            size="small"
            onClick={() => handleSendToIrz({ internalGroupId })}
          >
            Z grupą
          </Button>
        </>
      )}
    </Box>
  );
};

export default SendToIrzCell;
