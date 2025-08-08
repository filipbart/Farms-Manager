import React, { useEffect, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Grid,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { toast } from "react-toastify";
import dayjs from "dayjs";
import { MdSave } from "react-icons/md";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import AppDialog from "../../../common/app-dialog";
import LoadingButton from "../../../common/loading-button";
import { FallenStockPickupService } from "../../../../services/production-data/fallen-stock-pickups-service";
import LoadingTextField from "../../../common/loading-textfield";
import type { FallenStockPickupRow } from "../../../../models/fallen-stocks/fallen-stock-pickups";

interface EditFallenStockPickupModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  pickupToEdit: FallenStockPickupRow | undefined;
}

const EditFallenStockPickupModal: React.FC<EditFallenStockPickupModalProps> = ({
  open,
  onClose,
  onSave,
  pickupToEdit,
}) => {
  const [loading, setLoading] = useState(false);
  const [quantity, setQuantity] = useState<number | string>("");
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (pickupToEdit) {
      setQuantity(pickupToEdit.quantity);
    } else {
      setQuantity("");
      setError(null);
    }
  }, [pickupToEdit]);

  const validate = (): boolean => {
    const quantityNum = Number(quantity);
    if (!quantity || isNaN(quantityNum) || quantityNum <= 0) {
      setError("Ilość musi być większa od 0");
      return false;
    }
    setError(null);
    return true;
  };

  const handleSave = async () => {
    if (!validate() || !pickupToEdit) return;
    setLoading(true);

    await handleApiResponse(
      () =>
        FallenStockPickupService.updateFallenStockPickup(
          pickupToEdit.id,
          Number(quantity)
        ),
      () => {
        toast.success("Wpis odbioru został zaktualizowany");
        onSave();
        onClose();
      },
      undefined,
      "Nie udało się zaktualizować wpisu"
    );
    setLoading(false);
  };

  const handleClose = () => {
    setQuantity("");
    setError(null);
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Edytuj wpis odbioru</DialogTitle>
      <DialogContent>
        <Grid container spacing={2} sx={{ mt: 0.5 }}>
          <Grid size={{ xs: 12, sm: 6 }}>
            <LoadingTextField
              loading={loading}
              label="Ferma"
              value={pickupToEdit?.farmName ?? ""}
              fullWidth
              slotProps={{ input: { readOnly: true } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <LoadingTextField
              loading={loading}
              label="Cykl"
              value={pickupToEdit?.cycleText ?? ""}
              fullWidth
              slotProps={{ input: { readOnly: true } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <DatePicker
              label="Data odbioru"
              value={pickupToEdit?.date ? dayjs(pickupToEdit.date) : null}
              format="DD.MM.YYYY"
              readOnly
              slotProps={{
                textField: {
                  fullWidth: true,
                },
              }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Ilość [szt.]"
              type="number"
              value={quantity}
              onChange={(e) => setQuantity(e.target.value)}
              fullWidth
              error={!!error}
              helperText={error}
              slotProps={{ htmlInput: { min: 1 } }}
              disabled={loading}
              autoFocus
            />
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button disabled={loading} onClick={handleClose}>
          Anuluj
        </Button>
        <LoadingButton
          loading={loading}
          onClick={handleSave}
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
        >
          Zapisz zmiany
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default EditFallenStockPickupModal;
