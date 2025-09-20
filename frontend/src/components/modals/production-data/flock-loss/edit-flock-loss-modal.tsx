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
  ProductionDataFlockLossListModel,
  UpdateFlockLossData,
} from "../../../../models/production-data/flock-loss";
import { ProductionDataFlockLossService } from "../../../../services/production-data/flock-loss-measures-service";
import type CycleDto from "../../../../models/farms/latest-cycle";
import { FarmsService } from "../../../../services/farms-service";
import LoadingTextField from "../../../common/loading-textfield";

interface EditProductionDataFlockLossModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  flockLoss: ProductionDataFlockLossListModel | null;
}

type FormDataType = UpdateFlockLossData & { cycleId: string };

const EditProductionDataFlockLossModal: React.FC<
  EditProductionDataFlockLossModalProps
> = ({ open, onClose, onSave, flockLoss }) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
    setValue,
  } = useForm<FormDataType>();

  const [loading, setLoading] = useState(false);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);

  const selectedMeasureNumber = watch("measureNumber");
  const cycleId = watch("cycleId");

  useEffect(() => {
    if (flockLoss && open) {
      reset({
        cycleId: flockLoss.cycleId,
        measureNumber: undefined,
        day: undefined,
        quantity: undefined,
      });

      const fetchCycles = async () => {
        if (!flockLoss.farmId) return;
        setLoadingCycles(true);
        await handleApiResponse(
          () => FarmsService.getFarmCycles(flockLoss.farmId),
          (data) => setCycles(data.responseData ?? []),
          () => setCycles([]),
          "Nie udało się pobrać listy cykli."
        );
        setLoadingCycles(false);
      };

      fetchCycles();
    }
  }, [flockLoss, open, reset]);

  useEffect(() => {
    if (selectedMeasureNumber && flockLoss) {
      const dayKey =
        `flockLoss${selectedMeasureNumber}Day` as keyof ProductionDataFlockLossListModel;
      const quantityKey =
        `flockLoss${selectedMeasureNumber}Quantity` as keyof ProductionDataFlockLossListModel;

      const currentDay = flockLoss[dayKey] as number | undefined;
      const currentQuantity = flockLoss[quantityKey] as number | undefined;

      setValue("day", currentDay || 0);
      setValue("quantity", currentQuantity || 0);
    }
  }, [selectedMeasureNumber, flockLoss, setValue]);

  const handleSave = async (data: FormDataType) => {
    if (!flockLoss) return;
    setLoading(true);
    await handleApiResponse(
      () => ProductionDataFlockLossService.updateFlockLoss(flockLoss.id, data),
      () => {
        toast.success("Pomyślnie zaktualizowano wpis pomiaru");
        onSave();
        close();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji danych"
    );
    setLoading(false);
  };

  const close = () => {
    reset();
    setCycles([]);
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Edytuj wpis pomiaru</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
            <TextField
              label="Ferma"
              value={flockLoss?.farmName || ""}
              InputProps={{ readOnly: true }}
              fullWidth
            />
            <TextField
              label="Kurnik"
              value={flockLoss?.henhouseName || ""}
              InputProps={{ readOnly: true }}
              fullWidth
            />
            <LoadingTextField
              loading={loadingCycles}
              label="Cykl"
              select
              fullWidth
              disabled={
                !flockLoss?.farmId || loadingCycles || cycles.length === 0
              }
              value={cycleId || ""}
              error={!!errors.cycleId}
              helperText={errors.cycleId?.message}
              {...register("cycleId", { required: "Cykl jest wymagany" })}
            >
              {cycles.map((cycle) => (
                <MenuItem key={cycle.id} value={cycle.id}>
                  {`${cycle.identifier}/${cycle.year}`}
                </MenuItem>
              ))}
            </LoadingTextField>

            <Divider sx={{ my: 1 }} />
            <Typography variant="h6" sx={{ mb: -1 }}>
              Dodaj / Edytuj pomiar
            </Typography>

            <TextField
              select
              label="Numer pomiaru"
              defaultValue=""
              error={!!errors.measureNumber}
              helperText={errors.measureNumber?.message}
              {...register("measureNumber", {
                required: "Wybierz numer pomiaru",
              })}
              fullWidth
            >
              {[1, 2, 3, 4].map((num) => (
                <MenuItem key={num} value={num}>
                  Pomiar {num}
                </MenuItem>
              ))}
            </TextField>

            <TextField
              label="Doba pomiaru"
              type="number"
              value={watch("day") ?? ""}
              InputProps={{ inputProps: { min: 0 } }}
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
              label="Upadki i wybrakowania [szt.]"
              type="number"
              value={watch("quantity") ?? ""}
              InputProps={{ inputProps: { min: 0 } }}
              error={!!errors.quantity}
              helperText={errors.quantity?.message}
              {...register("quantity", {
                required: "Ilość jest wymagana",
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

export default EditProductionDataFlockLossModal;
