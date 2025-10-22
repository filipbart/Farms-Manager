import { useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
} from "@mui/material";

interface MarkInvoiceCompletedModalProps {
  open: boolean;
  onClose: () => void;
  onConfirm: (paymentDate: string, comment: string) => void;
}

const MarkInvoiceCompletedModal: React.FC<MarkInvoiceCompletedModalProps> = ({
  open,
  onClose,
  onConfirm,
}) => {
  const [paymentDate, setPaymentDate] = useState("");
  const [comment, setComment] = useState("");

  const handleConfirm = () => {
    if (!paymentDate) {
      return;
    }
    onConfirm(paymentDate, comment);
    setPaymentDate("");
    setComment("");
  };

  const handleClose = () => {
    setPaymentDate("");
    setComment("");
    onClose();
  };

  const isValid = paymentDate.trim() !== "";

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Oznacz fakturę jako zrealizowaną</DialogTitle>
      <DialogContent>
        <TextField
          autoFocus
          margin="dense"
          label="Data płatności *"
          type="date"
          fullWidth
          value={paymentDate}
          onChange={(e) => setPaymentDate(e.target.value)}
          InputLabelProps={{
            shrink: true,
          }}
          sx={{ mb: 2 }}
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
          disabled={!isValid}
        >
          Potwierdź
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default MarkInvoiceCompletedModal;
