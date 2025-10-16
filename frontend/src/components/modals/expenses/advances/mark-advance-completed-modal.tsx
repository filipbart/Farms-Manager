import { useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
} from "@mui/material";
import dayjs from "dayjs";

interface MarkAdvanceCompletedModalProps {
  open: boolean;
  onClose: () => void;
  onConfirm: (paymentDate: string, comment: string) => void;
}

const MarkAdvanceCompletedModal: React.FC<MarkAdvanceCompletedModalProps> = ({
  open,
  onClose,
  onConfirm,
}) => {
  const [paymentDate, setPaymentDate] = useState(
    dayjs().format("YYYY-MM-DD")
  );
  const [comment, setComment] = useState("");
  const [error, setError] = useState("");

  const handleConfirm = () => {
    if (!paymentDate) {
      setError("Data płatności jest obowiązkowa");
      return;
    }
    onConfirm(paymentDate, comment);
    setPaymentDate(dayjs().format("YYYY-MM-DD"));
    setComment("");
    setError("");
  };

  const handleClose = () => {
    setPaymentDate(dayjs().format("YYYY-MM-DD"));
    setComment("");
    setError("");
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Oznacz zaliczkę jako zrealizowaną</DialogTitle>
      <DialogContent>
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
          <TextField
            label="Data płatności"
            type="date"
            fullWidth
            required
            value={paymentDate}
            onChange={(e) => {
              setPaymentDate(e.target.value);
              setError("");
            }}
            error={!!error}
            helperText={error}
            slotProps={{
              inputLabel: {
                shrink: true,
              },
            }}
          />
          <TextField
            autoFocus
            label="Komentarz (opcjonalnie)"
            type="text"
            fullWidth
            multiline
            rows={3}
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Np. Przelew zrobiony 16.10"
          />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} color="inherit">
          Anuluj
        </Button>
        <Button onClick={handleConfirm} variant="contained" color="primary">
          Potwierdź
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default MarkAdvanceCompletedModal;
