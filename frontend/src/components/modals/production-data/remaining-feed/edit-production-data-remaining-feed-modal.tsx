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
import AppDialog from "../../../common/app-dialog";
import type CycleDto from "../../../../models/farms/latest-cycle";
import { FarmsService } from "../../../../services/farms-service";
import LoadingTextField from "../../../common/loading-textfield";
import type { ProductionDataRemainingFeedListModel } from "../../../../models/production-data/remaining-feed";
import { ProductionDataRemainingFeedService } from "../../../../services/production-data/production-data-remaining-feed-service";

interface EditProductionDataRemainingFeedModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  remainingFeed: ProductionDataRemainingFeedListModel | null;
}

const EditProductionDataRemainingFeedModal: React.FC<
  EditProductionDataRemainingFeedModalProps
> = ({ open, onClose, onSave, remainingFeed }) => {
  const [loading, setLoading] = useState(false);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [form, setForm] = useState({
    cycleId: "",
    remainingTonnage: "",
    remainingValue: "",
  });

  const [errors, setErrors] = useState({
    cycleId: "",
    remainingTonnage: "",
    remainingValue: "",
  });

  useEffect(() => {
    if (remainingFeed && open) {
      setForm({
        cycleId: remainingFeed.cycleId,
        remainingTonnage: String(remainingFeed.remainingTonnage),
        remainingValue: String(remainingFeed.remainingValue),
      });

      const fetchCycles = async () => {
        if (!remainingFeed.farmId) return;
        setLoadingCycles(true);
        await handleApiResponse(
          () => FarmsService.getFarmCycles(remainingFeed.farmId),
          (data) => setCycles(data.responseData ?? []),
          () => setCycles([]),
          "Nie udało się pobrać listy cykli."
        );
        setLoadingCycles(false);
      };

      fetchCycles();
    }
  }, [remainingFeed, open]);

  const validate = () => {
    const e = { cycleId: "", remainingTonnage: "", remainingValue: "" };

    if (!form.cycleId) {
      e.cycleId = "Cykl jest wymagany";
    }

    const tonnage = Number(form.remainingTonnage);
    if (isNaN(tonnage) || tonnage < 0)
      e.remainingTonnage = "Wartość nie może być ujemna";

    const value = Number(form.remainingValue);
    if (isNaN(value) || value < 0) {
      e.remainingValue = "Wartość nie może być ujemna";
    }

    setErrors(e);
    return Object.values(e).every((v) => !v);
  };

  const handleClose = () => {
    setErrors({ cycleId: "", remainingTonnage: "", remainingValue: "" });
    setCycles([]);
    onClose();
  };

  const handleSave = async () => {
    if (!remainingFeed?.id || !validate()) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        ProductionDataRemainingFeedService.updateRemainingFeed(
          remainingFeed.id,
          {
            cycleId: form.cycleId,
            remainingTonnage: Number(form.remainingTonnage),
            remainingValue: Number(form.remainingValue),
          }
        ),
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
      <DialogTitle>Edytuj wpis o pozostałej paszy</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
          <TextField
            label="Ferma"
            value={remainingFeed?.farmName || ""}
            slotProps={{ input: { readOnly: true } }}
            fullWidth
          />
          <TextField
            label="Kurnik"
            value={remainingFeed?.henhouseName || ""}
            slotProps={{ input: { readOnly: true } }}
            fullWidth
          />
          <LoadingTextField
            loading={loadingCycles}
            label="Cykl"
            select
            fullWidth
            disabled={
              !remainingFeed?.farmId || loadingCycles || cycles.length === 0
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
          <TextField
            label="Typ (nazwa) paszy"
            value={remainingFeed?.feedName || ""}
            slotProps={{ input: { readOnly: true } }}
            fullWidth
          />

          <TextField
            label="Tonaż pozostały [t]"
            value={form.remainingTonnage}
            onChange={(e) =>
              setForm((f) => ({ ...f, remainingTonnage: e.target.value }))
            }
            error={!!errors.remainingTonnage}
            helperText={errors.remainingTonnage}
            fullWidth
            type="number"
            slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
          />

          <TextField
            label="Wartość pozostała [zł]"
            value={form.remainingValue}
            onChange={(e) =>
              setForm((f) => ({ ...f, remainingValue: e.target.value }))
            }
            error={!!errors.remainingValue}
            helperText={errors.remainingValue}
            fullWidth
            type="number"
            slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
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

export default EditProductionDataRemainingFeedModal;
