import { Box, Button, Modal, Typography } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import dayjs, { type Dayjs } from "dayjs";
import { useState } from "react";
import LoadingButton from "../../../common/loading-button";

const modalStyle = {
  position: "absolute" as const,
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 400,
  bgcolor: "background.paper",
  boxShadow: 24,
  p: 4,
  borderRadius: 2,
};

interface BookPaymentModalProps {
  open: boolean;
  loading: boolean;
  onClose: () => void;
  onBookPayment: (paymentDate: string) => Promise<void>;
}

const BookPaymentModal: React.FC<BookPaymentModalProps> = ({
  open,
  loading,
  onClose,
  onBookPayment,
}) => {
  const [paymentDate, setPaymentDate] = useState<Dayjs | null>(dayjs());

  const handleBook = async () => {
    if (paymentDate) {
      const formattedDate = paymentDate.format("YYYY-MM-DD");
      await onBookPayment(formattedDate);
    }
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box sx={modalStyle}>
        <Typography variant="h6" component="h2" mb={2}>
          Zaksięguj płatność
        </Typography>

        <DatePicker
          label="Data płatności"
          format="DD.MM.YYYY"
          value={paymentDate}
          onChange={(newValue) => setPaymentDate(newValue)}
          sx={{ width: "100%" }}
          slotProps={{ textField: { required: true } }}
        />

        <Box
          sx={{
            mt: 3,
            display: "flex",
            justifyContent: "flex-end",
            alignItems: "center",
            gap: 1,
          }}
        >
          <Button onClick={onClose} disabled={loading}>
            Anuluj
          </Button>
          <LoadingButton
            onClick={handleBook}
            variant="contained"
            color="primary"
            loading={loading}
            disabled={loading || !paymentDate}
          >
            Zaksięguj
          </LoadingButton>
        </Box>
      </Box>
    </Modal>
  );
};

export default BookPaymentModal;
