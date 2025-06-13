import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Select,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";

interface SetCycleModalProps {
  open: boolean;
  onClose: () => void;
  onSave: (data: { farmId: string; cycle: string }) => void;
}

const SetCycleModal: React.FC<SetCycleModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const [form, setForm] = useState({
    farm: "",

    identifier: "",
  });

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSave = () => {
    onClose();
  };

  const farms = [
    { id: "1", name: "Ferma A" },
    { id: "2", name: "Ferma B" },
  ];

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Ustaw nowy cykl</DialogTitle>
      <DialogContent sx={{ mt: 1 }}>
        <Box display="flex" flexDirection="column" gap={3}>
          <Box mt={1}>
            <TextField
              select
              name="farm"
              label="Wybierz fermę"
              value={form.farm}
              onChange={handleChange}
              fullWidth
            >
              <MenuItem disabled value="">
                Wybierz Fermę
              </MenuItem>
              {farms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              ))}
            </TextField>
          </Box>

          <Box>
            <TextField
              name="identifier"
              label="Nowy cykl"
              value={form.identifier}
              onChange={handleChange}
              fullWidth
            />
          </Box>
        </Box>
      </DialogContent>
      <DialogActions>
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

export default SetCycleModal;
