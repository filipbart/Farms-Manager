import React, { useEffect, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  TextField,
  MenuItem,
} from "@mui/material";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import type { ProductionDataFailureListModel } from "../../../../models/production-data/failures";
import { ProductionDataFailuresService } from "../../../../services/production-data/production-data-failures-service";
import AppDialog from "../../../common/app-dialog";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";
import LoadingTextField from "../../../common/loading-textfield";

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
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [form, setForm] = useState({
    cycleId: "",
    deadCount: "",
    defectiveCount: "",
  });

  const [errors, setErrors] = useState({
    cycleId: "",
    deadCount: "",
    defectiveCount: "",
  });

  useEffect(() => {
    if (failure && open) {
      setForm({
        cycleId: failure.cycleId,
        deadCount: String(failure.deadCount),
        defectiveCount: String(failure.defectiveCount),
      });

      const fetchCycles = async () => {
        if (!failure.farmId) return;
        setLoadingCycles(true);
        await handleApiResponse(
          () => FarmsService.getFarmCycles(failure.farmId),
          (data) => setCycles(data.responseData ?? []),
          () => setCycles([]),
          "Nie udało się pobrać listy cykli."
        );
        setLoadingCycles(false);
      };

      fetchCycles();
    }
  }, [failure, open]);

  const validate = () => {
    const e = { deadCount: "", defectiveCount: "", cycleId: "" };

    if (!form.cycleId) {
      e.cycleId = "Cykl jest wymagany";
    }

    const dead = Number(form.deadCount);
    if (isNaN(dead) || dead < 0) e.deadCount = "Wartość nie może być ujemna";

    const defective = Number(form.defectiveCount);
    if (isNaN(defective) || defective < 0)
      e.defectiveCount = "Wartość nie może być ujemna";

    setErrors(e);
    return Object.values(e).every((v) => !v);
  };

  const handleClose = () => {
    setErrors({ cycleId: "", deadCount: "", defectiveCount: "" });
    setCycles([]);
    onClose();
  };

  const handleSave = async () => {
    if (!failure?.id || !validate()) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        ProductionDataFailuresService.updateFailure(failure.id, {
          cycleId: form.cycleId,
          deadCount: Number(form.deadCount),
          defectiveCount: Number(form.defectiveCount),
        }),
      () => {
        toast.success("Zaktualizowano wpis");
        onSave();
        handleClose();
      },
      undefined,
      "Nie udało się zapisać zmian"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
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
          <LoadingTextField
            loading={loadingCycles}
            label="Cykl"
            select
            fullWidth
            disabled={!failure?.farmId || loadingCycles || cycles.length === 0}
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

          <TextField
            label="Upadki [szt.]"
            value={form.deadCount}
            onChange={(e) =>
              setForm((f) => ({ ...f, deadCount: e.target.value }))
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
              setForm((f) => ({ ...f, defectiveCount: e.target.value }))
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
        <Button onClick={handleClose}>Anuluj</Button>
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
