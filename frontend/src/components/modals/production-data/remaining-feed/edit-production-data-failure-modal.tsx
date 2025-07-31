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
import AppDialog from "../../../common/app-dialog";
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
  const [form, setForm] = useState({
    remainingTonnage: 0,
    remainingValue: 0,
  });

  const [errors, setErrors] = useState({
    remainingTonnage: "",
    remainingValue: "",
  });

  useEffect(() => {
    if (remainingFeed) {
      setForm({
        remainingTonnage: remainingFeed.remainingTonnage,
        remainingValue: remainingFeed.remainingValue,
      });
    }
  }, [remainingFeed]);

  const validate = () => {
    const e = { remainingTonnage: "", remainingValue: "" };

    const tonnage = Number(form.remainingTonnage);
    if (isNaN(tonnage) || tonnage < 0)
      e.remainingTonnage = "Wartość nie może być ujemna";

    const value = Number(form.remainingValue);
    if (isNaN(value) || value < 0)
      e.remainingValue = "Wartość nie może być ujemna";

    setErrors(e);
    return Object.values(e).every((v) => !v);
  };

  const handleSave = async () => {
    if (!remainingFeed?.id || !validate()) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        ProductionDataRemainingFeedService.updateRemainingFeed(
          remainingFeed.id,
          {
            remainingTonnage: Number(form.remainingTonnage),
            remainingValue: Number(form.remainingValue),
          }
        ),
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
          <TextField
            label="Cykl"
            value={remainingFeed?.cycleText || ""}
            slotProps={{ input: { readOnly: true } }}
            fullWidth
          />
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
              setForm((f) => ({
                ...f,
                remainingTonnage: Number(e.target.value),
              }))
            }
            error={!!errors.remainingTonnage}
            helperText={errors.remainingTonnage}
            fullWidth
            type="number"
            InputProps={{ inputProps: { min: 0, step: "0.01" } }}
          />

          <TextField
            label="Wartość pozostała [zł]"
            value={form.remainingValue}
            onChange={(e) =>
              setForm((f) => ({
                ...f,
                remainingValue: Number(e.target.value),
              }))
            }
            error={!!errors.remainingValue}
            helperText={errors.remainingValue}
            fullWidth
            type="number"
            InputProps={{ inputProps: { min: 0, step: "0.01" } }}
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

export default EditProductionDataRemainingFeedModal;
