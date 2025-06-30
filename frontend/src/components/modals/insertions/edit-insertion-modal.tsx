import React, { useEffect, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  TextField,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import { toast } from "react-toastify";
import LoadingButton from "../../common/loading-button";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { InsertionsService } from "../../../services/insertions-service";
import type { Dayjs } from "dayjs";
import type InsertionListModel from "../../../models/insertions/insertions";
import dayjs from "dayjs";

interface EditInsertionModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  insertion: InsertionListModel | null;
}

const EditInsertionModal: React.FC<EditInsertionModalProps> = ({
  open,
  onClose,
  onSave,
  insertion,
}) => {
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    insertionDate: null as Dayjs | null,
    quantity: "",
    bodyWeight: "",
  });

  const [errors, setErrors] = useState({
    insertionDate: "",
    quantity: "",
    bodyWeight: "",
  });

  useEffect(() => {
    if (open && insertion) {
      setForm({
        insertionDate: dayjs(insertion.insertionDate),
        quantity: insertion.quantity.toString(),
        bodyWeight: insertion.bodyWeight.toString(),
      });
    }
  }, [open, insertion]);

  const validate = () => {
    const e = { insertionDate: "", quantity: "", bodyWeight: "" };

    if (!form.insertionDate) e.insertionDate = "Wymagana";

    const qty = Number(form.quantity);
    if (isNaN(qty) || qty <= 0) e.quantity = "Musi być większa niż 0";

    const bw = Number(form.bodyWeight);
    if (isNaN(bw) || bw <= 0) e.bodyWeight = "Musi być większa niż 0";

    setErrors(e);
    return Object.values(e).every((v) => !v);
  };

  const handleSave = async () => {
    if (!insertion?.id || !validate()) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        InsertionsService.updateInsertion(insertion.id, {
          insertionDate: form.insertionDate!.format("YYYY-MM-DD"),
          quantity: Number(form.quantity),
          bodyWeight: Number(form.bodyWeight),
        }),
      () => {
        toast.success("Zaktualizowano wstawienie");
        onSave();
        onClose();
      },
      undefined,
      "Nie udało się zapisać zmian"
    );
    setLoading(false);
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Edycja wstawienia</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} mt={1}>
          <DatePicker
            label="Data wstawienia"
            value={form.insertionDate}
            onChange={(value) =>
              setForm((f) => ({ ...f, insertionDate: value }))
            }
            disableFuture
            format="DD.MM.YYYY"
            slotProps={{
              textField: {
                error: !!errors.insertionDate,
                helperText: errors.insertionDate,
                fullWidth: true,
              },
            }}
          />
          <TextField
            label="Sztuki wstawione"
            value={form.quantity}
            onChange={(e) =>
              setForm((f) => ({ ...f, quantity: e.target.value }))
            }
            error={!!errors.quantity}
            helperText={errors.quantity}
            fullWidth
            type="number"
          />
          <TextField
            label="Śr. masa ciała"
            value={form.bodyWeight}
            onChange={(e) =>
              setForm((f) => ({ ...f, bodyWeight: e.target.value }))
            }
            error={!!errors.bodyWeight}
            helperText={errors.bodyWeight}
            fullWidth
            type="number"
          />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Anuluj</Button>
        <LoadingButton
          loading={loading}
          onClick={handleSave}
          variant="contained"
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </Dialog>
  );
};

export default EditInsertionModal;
