import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Box,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { useState } from "react";
import { Dayjs } from "dayjs";

interface AddInsertionModalProps {
  open: boolean;
  onClose: () => void;
}

const AddInsertionModal: React.FC<AddInsertionModalProps> = ({
  open,
  onClose,
}) => {
  const [form, setForm] = useState({
    farm: "",
    barn: "",
    identifier: "",
    insertionDate: null as Dayjs | null,
    quantity: "",
    hatchery: "",
    bodyWeight: "",
  });

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSave = () => {
    console.log("Saving form:", form);
    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Wprowadź Dane Wstawienia</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} mt={1}>
          <TextField
            select
            name="farm"
            label="Ferma"
            value={form.farm}
            onChange={handleChange}
            fullWidth
          >
            <MenuItem disabled value="">
              Wybierz fermę
            </MenuItem>
            <MenuItem value="ferma1">Ferma 1</MenuItem>
            <MenuItem value="ferma2">Ferma 2</MenuItem>
          </TextField>

          <TextField
            select
            name="barn"
            label="Kurnik"
            value={form.barn}
            onChange={handleChange}
            fullWidth
          >
            <MenuItem disabled value="">
              Wybierz kurnik
            </MenuItem>
            <MenuItem value="kurnik1">Kurnik 1</MenuItem>
            <MenuItem value="kurnik2">Kurnik 2</MenuItem>
          </TextField>

          <TextField
            name="identifier"
            label="Identyfikator"
            value={form.identifier}
            onChange={handleChange}
            fullWidth
          />

          <DatePicker
            label="Data wstawienia"
            value={form.insertionDate}
            disableFuture
            onChange={(newValue) =>
              setForm((prev) => ({ ...prev, insertionDate: newValue }))
            }
            format="DD.MM.YYYY"
          />

          <TextField
            name="quantity"
            label="Sztuki wstawione"
            value={form.quantity}
            onChange={handleChange}
            fullWidth
          />

          <TextField
            select
            name="hatchery"
            label="Wylęgarnia"
            value={form.hatchery}
            onChange={handleChange}
            fullWidth
          >
            <MenuItem disabled value="">
              Wybierz wylęgarnię
            </MenuItem>
            <MenuItem value="wyl1">Wylęgarnia 1</MenuItem>
            <MenuItem value="wyl2">Wylęgarnia 2</MenuItem>
          </TextField>

          <TextField
            name="bodyWeight"
            label="Śr. masa ciała"
            value={form.bodyWeight}
            onChange={handleChange}
            fullWidth
          />
        </Box>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button onClick={onClose} variant="outlined" color="inherit">
          Anuluj
        </Button>
        <Button onClick={handleSave} variant="contained" color="primary">
          Zapisz
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AddInsertionModal;
