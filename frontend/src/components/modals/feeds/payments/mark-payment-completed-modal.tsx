import { useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs, { Dayjs } from "dayjs";
import "dayjs/locale/pl";

interface MarkPaymentCompletedModalProps {
  open: boolean;
  onClose: () => void;
  onConfirm: (comment: string, paymentDate: string) => void;
}

const MarkPaymentCompletedModal: React.FC<MarkPaymentCompletedModalProps> = ({
  open,
  onClose,
  onConfirm,
}) => {
  const [comment, setComment] = useState("");
  const [paymentDate, setPaymentDate] = useState<Dayjs | null>(dayjs());

  const handleConfirm = () => {
    if (!paymentDate) return;
    onConfirm(comment, paymentDate.format("YYYY-MM-DD"));
    setComment("");
    setPaymentDate(dayjs());
  };

  const handleClose = () => {
    setComment("");
    setPaymentDate(dayjs());
    onClose();
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale="pl">
      <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
        <DialogTitle>Oznacz przelew jako zrealizowany</DialogTitle>
        <DialogContent>
          <DatePicker
            label="Data płatności *"
            value={paymentDate}
            onChange={(newValue) => setPaymentDate(newValue)}
            format="DD.MM.YYYY"
            slotProps={{
              textField: {
                fullWidth: true,
                margin: "dense",
                required: true,
              },
            }}
          />
          <TextField
            margin="dense"
            label="Komentarz (opcjonalnie)"
            type="text"
            fullWidth
            multiline
            rows={3}
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Np. Przelew zrobiony 16.10"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="inherit">
            Anuluj
          </Button>
          <Button
            onClick={handleConfirm}
            variant="contained"
            color="primary"
            disabled={!paymentDate}
          >
            Potwierdź
          </Button>
        </DialogActions>
      </Dialog>
    </LocalizationProvider>
  );
};

export default MarkPaymentCompletedModal;
