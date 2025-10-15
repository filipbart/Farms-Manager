import { useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
} from "@mui/material";

interface MarkPaymentCompletedModalProps {
  open: boolean;
  onClose: () => void;
  onConfirm: (comment: string) => void;
}

const MarkPaymentCompletedModal: React.FC<MarkPaymentCompletedModalProps> = ({
  open,
  onClose,
  onConfirm,
}) => {
  const [comment, setComment] = useState("");

  const handleConfirm = () => {
    onConfirm(comment);
    setComment("");
  };

  const handleClose = () => {
    setComment("");
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Oznacz przelew jako zrealizowany</DialogTitle>
      <DialogContent>
        <TextField
          autoFocus
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
        <Button onClick={handleConfirm} variant="contained" color="primary">
          Potwierdź
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default MarkPaymentCompletedModal;
