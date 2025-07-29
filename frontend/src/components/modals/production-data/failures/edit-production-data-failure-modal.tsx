import React, { useEffect, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  TextField,
} from "@mui/material";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import type { ProductionDataFailureListModel } from "../../../../models/production-data/failures";
import { ProductionDataFailuresService } from "../../../../services/production-data/production-data-failures-service";
import AppDialog from "../../../common/app-dialog";

interface EditProductionDataFailureModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  failure: ProductionDataFailureListModel | null;
}

const EditProductionDataFailureModal: React.FC<
  EditProductionDataFailureModalProps
> = ({ open, onClose, onSave, failure }) => {
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    deadCount: 0,
    defectiveCount: 0,
  });

  const [errors, setErrors] = useState({
    deadCount: "",
    defectiveCount: "",
  });

  useEffect(() => {
    if (failure) {
      setForm({
        deadCount: failure.deadCount,
        defectiveCount: failure.defectiveCount,
      });
    }
  }, [failure]);

  const validate = () => {
    const e = { deadCount: "", defectiveCount: "" };

    const dead = Number(form.deadCount);
    if (isNaN(dead) || dead < 0) e.deadCount = "Wartość nie może być ujemna";

    const defective = Number(form.defectiveCount);
    if (isNaN(defective) || defective < 0)
      e.defectiveCount = "Wartość не może być ujemna";

    setErrors(e);
    return Object.values(e).every((v) => !v);
  };

  const handleSave = async () => {
    if (!failure?.id || !validate()) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        ProductionDataFailuresService.updateFailure(failure.id, {
          deadCount: Number(form.deadCount),
          defectiveCount: Number(form.defectiveCount),
        }),
      () => {
        toast.success("Zaktualizowano wpis");
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
      <DialogTitle>Edytuj wpis o upadkach i wybrakowaniach</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
          <TextField
            label="Ferma"
            value={failure?.farmName || ""}
            slotProps={{ input: { readOnly: true } }}
            fullWidth
          />
          <TextField
            label="Kurnik"
            value={failure?.henhouseName || ""}
            slotProps={{ input: { readOnly: true } }}
            fullWidth
          />
          <TextField
            label="Cykl"
            value={failure?.cycleText || ""}
            slotProps={{ input: { readOnly: true } }}
            fullWidth
          />

          <TextField
            label="Upadki [szt.]"
            value={form.deadCount}
            onChange={(e) =>
              setForm((f) => ({ ...f, deadCount: Number(e.target.value) }))
            }
            error={!!errors.deadCount}
            helperText={errors.deadCount}
            fullWidth
            type="number"
            slotProps={{ htmlInput: { min: 0 } }}
          />

          <TextField
            label="Wybrakowania [szt.]"
            value={form.defectiveCount}
            onChange={(e) =>
              setForm((f) => ({
                ...f,
                defectiveCount: Number(e.target.value),
              }))
            }
            error={!!errors.defectiveCount}
            helperText={errors.defectiveCount}
            fullWidth
            type="number"
            slotProps={{ htmlInput: { min: 0 } }}
          />
        </Box>
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

export default EditProductionDataFailureModal;
