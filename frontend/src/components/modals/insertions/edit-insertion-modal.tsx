import React, { useEffect, useMemo, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Grid,
  TextField,
  MenuItem,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import { toast } from "react-toastify";
import LoadingButton from "../../common/loading-button";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { InsertionsService } from "../../../services/insertions-service";
import type { Dayjs } from "dayjs";
import type InsertionListModel from "../../../models/insertions/insertions";
import dayjs from "dayjs";
import { MdSave } from "react-icons/md";
import AppDialog from "../../common/app-dialog";
import { useFarms } from "../../../hooks/useFarms";
import { FarmsService } from "../../../services/farms-service";
import type CycleDto from "../../../models/farms/latest-cycle";
import LoadingTextField from "../../common/loading-textfield";

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
  const { farms, fetchFarms } = useFarms();
  const [loading, setLoading] = useState(false);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);

  const [form, setForm] = useState({
    farmId: "",
    cycleId: "",
    insertionDate: null as Dayjs | null,
    quantity: "",
    bodyWeight: "",
    comment: "",
  });

  const farmName = useMemo(() => {
    if (!insertion || !farms.length) return "";
    return farms.find((f) => f.id === insertion.farmId)?.name || "";
  }, [insertion, farms]);

  const [errors, setErrors] = useState({
    farmId: "",
    cycleId: "",
    insertionDate: "",
    quantity: "",
    bodyWeight: "",
  });

  useEffect(() => {
    if (open) {
      fetchFarms();
    }
  }, [open, fetchFarms]);

  useEffect(() => {
    if (open && insertion) {
      setForm({
        farmId: insertion.farmId,
        cycleId: insertion.cycleId,
        insertionDate: dayjs(insertion.insertionDate),
        quantity: insertion.quantity.toString(),
        bodyWeight: insertion.bodyWeight.toString(),
        comment: insertion.comment || "",
      });
    }
  }, [open, insertion]);

  useEffect(() => {
    const fetchCycles = async (farmId: string) => {
      if (!farmId) {
        setCycles([]);
        return;
      }
      setLoadingCycles(true);
      await handleApiResponse(
        () => FarmsService.getFarmCycles(farmId),
        (data) => {
          setCycles(data.responseData ?? []);
          setLoadingCycles(false);
        },
        () => {
          setCycles([]);
          setLoadingCycles(false);
        },
        "Nie udało się pobrać cykli dla wybranej fermy."
      );
    };

    if (form.farmId) {
      fetchCycles(form.farmId);
    }
  }, [form.farmId]);

  const validate = () => {
    const e = {
      farmId: "",
      cycleId: "",
      insertionDate: "",
      quantity: "",
      bodyWeight: "",
    };

    if (!form.farmId) e.farmId = "Wymagana";
    if (!form.cycleId) e.cycleId = "Wymagany";

    if (!form.insertionDate) e.insertionDate = "Wymagana";

    const qty = Number(form.quantity);
    if (form.quantity === "" || isNaN(qty) || qty <= 0)
      e.quantity = "Musi być większa niż 0";

    const bw = Number(form.bodyWeight);
    if (form.bodyWeight === "" || isNaN(bw) || bw <= 0)
      e.bodyWeight = "Musi być większa niż 0";

    setErrors(e);
    return Object.values(e).every((v) => !v);
  };

  const handleSave = async () => {
    if (!insertion?.id || !validate()) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        InsertionsService.updateInsertion(insertion.id, {
          farmId: form.farmId,
          cycleId: form.cycleId,
          insertionDate: form.insertionDate!.format("YYYY-MM-DD"),
          quantity: Number(form.quantity),
          bodyWeight: Number(form.bodyWeight),
          comment: form.comment || undefined,
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
    <AppDialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Edycja wstawienia</DialogTitle>
      <DialogContent>
        <Grid container spacing={2} sx={{ mt: 1 }}>
          <Grid size={12}>
            <TextField label="Ferma" fullWidth value={farmName} disabled />
          </Grid>
          <Grid size={12}>
            <LoadingTextField
              label="Cykl"
              select
              fullWidth
              loading={loadingCycles}
              value={form.cycleId}
              onChange={(e) =>
                setForm((f) => ({ ...f, cycleId: e.target.value }))
              }
              error={!!errors.cycleId}
              helperText={errors.cycleId}
              disabled={!form.farmId || loadingCycles || cycles.length === 0}
            >
              {cycles.map((cycle) => (
                <MenuItem key={cycle.id} value={cycle.id}>
                  {`${cycle.identifier}/${cycle.year}`}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Grid>
          <Grid size={12}>
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
          </Grid>
          <Grid size={12}>
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
          </Grid>
          <Grid size={12}>
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
          </Grid>
          <Grid size={12}>
            <TextField
              label="Komentarz"
              value={form.comment}
              onChange={(e) =>
                setForm((f) => ({ ...f, comment: e.target.value }))
              }
              fullWidth
              multiline
              rows={3}
              placeholder="Opcjonalny komentarz do wstawienia"
            />
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Anuluj</Button>
        <LoadingButton
          loading={loading}
          onClick={handleSave}
          variant="contained"
          startIcon={<MdSave />}
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default EditInsertionModal;
