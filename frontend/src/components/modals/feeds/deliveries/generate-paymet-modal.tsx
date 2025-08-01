import { useState } from "react";
import {
  DialogActions,
  DialogContent,
  DialogTitle,
  Button,
  TextField,
} from "@mui/material";
import AppDialog from "../../../common/app-dialog";

interface GeneratePaymentModalProps {
  open: boolean;
  onClose: () => void;
  onGenerate: (comment: string) => void;
}

const GeneratePaymentModal: React.FC<GeneratePaymentModalProps> = ({
  open,
  onClose,
  onGenerate,
}) => {
  const [comment, setComment] = useState("");

  const handleGenerate = () => {
    onGenerate(comment);
    setComment("");
  };

  const handleClose = () => {
    setComment("");
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Komentarz do przelewu (opcjonalny)</DialogTitle>
      <DialogContent>
        <TextField
          label="Komentarz"
          fullWidth
          multiline
          minRows={3}
          value={comment}
          onChange={(e) => setComment(e.target.value)}
          margin="normal"
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Anuluj</Button>
        <Button onClick={handleGenerate} variant="contained" color="primary">
          Generuj przelew
        </Button>
      </DialogActions>
    </AppDialog>
  );
};

export default GeneratePaymentModal;
