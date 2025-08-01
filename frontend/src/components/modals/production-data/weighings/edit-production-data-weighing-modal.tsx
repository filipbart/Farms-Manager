import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  MenuItem,
  Typography,
  Divider,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import AppDialog from "../../../common/app-dialog";
import type {
  ProductionDataWeighingListModel,
  UpdateWeighingData,
} from "../../../../models/production-data/weighings";
import { ProductionDataWeighingsService } from "../../../../services/production-data/production-data-weighings-service";

interface EditProductionDataWeighingModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  weighing: ProductionDataWeighingListModel | null;
}

const EditProductionDataWeighingModal: React.FC<
  EditProductionDataWeighingModalProps
> = ({ open, onClose, onSave, weighing }) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
    setValue,
  } = useForm<UpdateWeighingData>();

  const [loading, setLoading] = useState(false);

  const selectedWeighingNumber = watch("weighingNumber");

  useEffect(() => {
    if (weighing) {
      reset({
        weighingNumber: undefined,
        day: undefined,
        weight: undefined,
      });
    }
  }, [weighing, reset]);

  useEffect(() => {
    if (selectedWeighingNumber && weighing) {
      const dayKey =
        `weighing${selectedWeighingNumber}Day` as keyof ProductionDataWeighingListModel;
      const weightKey =
        `weighing${selectedWeighingNumber}Weight` as keyof ProductionDataWeighingListModel;

      const currentDay = weighing[dayKey] as number | undefined;
      const currentWeight = weighing[weightKey] as number | undefined;

      setValue("day", currentDay || 0);
      setValue("weight", currentWeight || 0);
    }
  }, [selectedWeighingNumber, weighing, setValue]);

  const handleSave = async (data: UpdateWeighingData) => {
    if (!weighing) return;
    setLoading(true);
    await handleApiResponse(
      () => ProductionDataWeighingsService.updateWeighing(weighing.id, data),
      () => {
        toast.success("Pomyślnie zaktualizowano wpis ważenia");
        onSave();
        onClose();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji danych"
    );
    setLoading(false);
  };

  const close = () => {
    reset();
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Edytuj wpis ważenia</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
            <TextField
              label="Ferma"
              value={weighing?.farmName || ""}
              slotProps={{ input: { readOnly: true } }}
              fullWidth
            />
            <TextField
              label="Kurnik"
              value={weighing?.henhouseName || ""}
              slotProps={{ input: { readOnly: true } }}
              fullWidth
            />
            <TextField
              label="Cykl"
              value={weighing?.cycleText || ""}
              slotProps={{ input: { readOnly: true } }}
              fullWidth
            />
            <TextField
              label="Wylęgarnia"
              value={weighing?.hatcheryName || ""}
              slotProps={{ input: { readOnly: true } }}
              fullWidth
            />

            <Divider sx={{ my: 1 }} />
            <Typography variant="h6" sx={{ mb: -1 }}>
              Dodaj / Edytuj ważenie
            </Typography>

            <TextField
              select
              label="Numer ważenia"
              defaultValue=""
              error={!!errors.weighingNumber}
              helperText={errors.weighingNumber?.message}
              {...register("weighingNumber", {
                required: "Wybierz numer ważenia",
              })}
              fullWidth
            >
              {[1, 2, 3, 4, 5].map((num) => (
                <MenuItem key={num} value={num}>
                  Ważenie {num}
                </MenuItem>
              ))}
            </TextField>

            <TextField
              label="Doba ważenia"
              type="number"
              value={watch("day") ?? ""}
              slotProps={{ htmlInput: { min: 0 } }}
              error={!!errors.day}
              helperText={errors.day?.message}
              {...register("day", {
                required: "Doba jest wymagana",
                valueAsNumber: true,
                min: { value: 0, message: "Doba nie może być ujemna" },
              })}
              fullWidth
            />
            <TextField
              label="Średnia masa ciała [g]"
              type="number"
              value={watch("weight") ?? ""}
              slotProps={{ htmlInput: { min: 0 } }}
              error={!!errors.weight}
              helperText={errors.weight?.message}
              {...register("weight", {
                required: "Masa ciała jest wymagana",
                valueAsNumber: true,
                min: { value: 0, message: "Wartość nie może być ujemna" },
              })}
              fullWidth
            />
          </Box>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button
            onClick={close}
            variant="outlined"
            color="inherit"
            disabled={loading}
          >
            Anuluj
          </Button>
          <LoadingButton
            type="submit"
            variant="contained"
            color="primary"
            startIcon={<MdSave />}
            disabled={loading}
            loading={loading}
          >
            Zapisz zmiany
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default EditProductionDataWeighingModal;
