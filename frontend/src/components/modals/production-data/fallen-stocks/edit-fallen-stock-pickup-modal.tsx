import React, { useEffect, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Grid,
  MenuItem,
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
import type CycleDto from "../../../../models/farms/latest-cycle";
import { FarmsService } from "../../../../services/farms-service";

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
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [form, setForm] = useState({
    cycleId: "",
    quantity: "",
  });
  const [errors, setErrors] = useState({
    cycleId: "",
    quantity: "",
  });

  useEffect(() => {
    if (pickupToEdit && open) {
      setForm({
        cycleId: pickupToEdit.cycleId,
        quantity: String(pickupToEdit.quantity),
      });

      const fetchCycles = async () => {
        if (!pickupToEdit.farmId) return;
        setLoadingCycles(true);
        await handleApiResponse(
          () => FarmsService.getFarmCycles(pickupToEdit.farmId),
          (data) => setCycles(data.responseData ?? []),
          () => setCycles([]),
          "Nie udało się pobrać listy cykli."
        );
        setLoadingCycles(false);
      };

      fetchCycles();
    }
  }, [pickupToEdit, open]);

  const validate = (): boolean => {
    const newErrors = { cycleId: "", quantity: "" };
    if (!form.cycleId) {
      newErrors.cycleId = "Cykl jest wymagany";
    }
    const quantityNum = Number(form.quantity);
    if (!form.quantity || isNaN(quantityNum) || quantityNum <= 0) {
      newErrors.quantity = "Ilość musi być większa od 0";
    }
    setErrors(newErrors);
    return Object.values(newErrors).every((v) => !v);
  };

  const handleSave = async () => {
    if (!validate() || !pickupToEdit) return;
    setLoading(true);

    await handleApiResponse(
      () =>
        FallenStockPickupService.updateFallenStockPickup(
          pickupToEdit.id,
          form.cycleId,
          Number(form.quantity)
        ),
      () => {
        toast.success("Wpis odbioru został zaktualizowany");
        onSave();
        handleClose();
      },
      undefined,
      "Nie udało się zaktualizować wpisu"
    );
    setLoading(false);
  };

  const handleClose = () => {
    setForm({ cycleId: "", quantity: "" });
    setErrors({ cycleId: "", quantity: "" });
    setCycles([]);
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Edytuj wpis odbioru</DialogTitle>
      <DialogContent>
        <Grid container spacing={2} sx={{ mt: 0.5 }}>
          <Grid size={{ xs: 12, sm: 6 }}>
            <LoadingTextField
              loading={false}
              label="Ferma"
              value={pickupToEdit?.farmName ?? ""}
              fullWidth
              slotProps={{ input: { readOnly: true } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <LoadingTextField
              loading={loadingCycles}
              label="Cykl"
              select
              fullWidth
              disabled={
                !pickupToEdit?.farmId || loadingCycles || cycles.length === 0
              }
              value={form.cycleId}
              onChange={(e) =>
                setForm((f) => ({ ...f, cycleId: e.target.value }))
              }
              error={!!errors.cycleId}
              helperText={errors.cycleId}
            >
              {cycles.map((cycle) => (
                <MenuItem key={cycle.id} value={cycle.id}>
                  {`${cycle.identifier}/${cycle.year}`}
                </MenuItem>
              ))}
            </LoadingTextField>
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
              value={form.quantity}
              onChange={(e) =>
                setForm((f) => ({ ...f, quantity: e.target.value }))
              }
              fullWidth
              error={!!errors.quantity}
              helperText={errors.quantity}
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
