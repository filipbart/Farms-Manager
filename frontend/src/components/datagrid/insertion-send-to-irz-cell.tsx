import { useState } from "react";
import { Box, Button, Typography } from "@mui/material";
import { toast } from "react-toastify";
import { InsertionsService } from "../../services/insertions-service";
import Loading from "../loading/loading";
import { handleApiResponse } from "../../utils/axios/handle-api-response";

interface InsertionSendToIrzCellProps {
  isSentToIrz: boolean;
  dateIrzSentUtc: string | null;
  insertionId: string;
  internalGroupId?: string;
  dispatch: any;
  filters: any;
}

const InsertionSendToIrzCell: React.FC<InsertionSendToIrzCellProps> = ({
  isSentToIrz,
  dateIrzSentUtc,
  insertionId,
  internalGroupId,
  dispatch,
  filters,
}) => {
  const [loading, setLoading] = useState(false);

  const handleSendToIrz = async (data: {
    insertionId?: string;
    internalGroupId?: string;
  }) => {
    setLoading(true);
    await handleApiResponse(
      () => InsertionsService.sendToIrzPlus(data),
      async () => {
        toast.success("Wysłano do IRZplus");
        dispatch({
          type: "setMultiple",
          payload: { page: filters.page },
        });
        setLoading(false);
      },
      undefined,
      "Wystąpił błąd podczas wysyłania do IRZplus"
    );
    setLoading(false);
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

  if (isSentToIrz) return null;

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "row",
        alignItems: "center",
        justifyContent: "center",
        gap: 1,
        flexWrap: "nowrap",
        height: "100%",
      }}
    >
      {loading ? (
        <Loading height="0" size={10} />
      ) : (
        <>
          <Button
            variant="contained"
            color="error"
            size="small"
            onClick={() => handleSendToIrz({ insertionId })}
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

export default InsertionSendToIrzCell;
