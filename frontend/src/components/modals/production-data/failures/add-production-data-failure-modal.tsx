import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  MenuItem,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { MdSave } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import type { AddProductionDataFailureData } from "../../../../models/production-data/failures";
import { ProductionDataFailuresService } from "../../../../services/production-data/production-data-failures-service";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import AppDialog from "../../../common/app-dialog";

interface AddProductionDataFailureModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddProductionDataFailureModal: React.FC<
  AddProductionDataFailureModalProps
> = ({ open, onClose, onSave }) => {
  const {
    register,
    handleSubmit,

    formState: { errors },
    reset,
    setValue,
    setError,
    clearErrors,
    watch,
  } = useForm<AddProductionDataFailureData & { cycleDisplay: string }>();

  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();
  const [availableHenhouses, setAvailableHenhouses] = useState<HouseRowModel[]>(
    []
  );

  useEffect(() => {
    if (open) {
      fetchFarms();
    }
  }, [open, fetchFarms]);

  const handleFarmChange = async (farmId: string) => {
    setValue("cycleId", "");
    setValue("cycleDisplay", "");
    setValue("henhouseId", "");
    clearErrors("cycleId");
    setAvailableHenhouses([]);

    const selectedFarm = farms.find((f) => f.id === farmId);
    if (selectedFarm) {
      setAvailableHenhouses(selectedFarm.henhouses);
    }

    const cycle = await loadLatestCycle(farmId);
    if (cycle) {
      setValue("cycleId", cycle.id);
      setValue("cycleDisplay", `${cycle.identifier}/${cycle.year}`);
      clearErrors("cycleId");
    } else {
      setError("cycleId", {
        type: "manual",
        message: "Brak aktywnego cyklu",
      });
    }
  };

  const handleSave = async (data: AddProductionDataFailureData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => ProductionDataFailuresService.addFailure(data),
      () => {
        onSave();
        close();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania danych"
    );
    setLoading(false);
  };

  const close = () => {
    reset();
    setAvailableHenhouses([]);
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Dodaj wpis o upadkach i wybrakowaniach</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
            <LoadingTextField
              loading={loadingFarms}
              select
              label="Ferma"
              fullWidth
              error={!!errors.farmId}
              helperText={errors.farmId?.message}
              {...register("farmId", {
                required: "Farma jest wymagana",
                onChange: (e) => handleFarmChange(e.target.value),
              })}
            >
              {farms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              ))}
            </LoadingTextField>

            <LoadingTextField
              loading={loadingCycle}
              label="Cykl"
              value={watch("cycleDisplay") || ""}
              slotProps={{ input: { readOnly: true } }}
              error={!!errors.cycleId}
              helperText={errors.cycleId?.message}
              fullWidth
            />

            <TextField
              select
              label="Kurnik"
              fullWidth
              disabled={!watch("farmId") || availableHenhouses.length === 0}
              error={!!errors.henhouseId}
              helperText={errors.henhouseId?.message}
              {...register("henhouseId", {
                required: "Kurnik jest wymagany",
              })}
            >
              {availableHenhouses.map((henhouse) => (
                <MenuItem key={henhouse.id} value={henhouse.id}>
                  {henhouse.name}
                </MenuItem>
              ))}
            </TextField>

            <TextField
              label="Upadki [szt.]"
              type="number"
              InputProps={{ inputProps: { min: 0 } }}
              error={!!errors.deadCount}
              helperText={errors.deadCount?.message}
              {...register("deadCount", {
                required: "Liczba upadków jest wymagana",
                valueAsNumber: true,
                min: { value: 0, message: "Wartość nie może być ujemna" },
              })}
              fullWidth
            />
            <TextField
              label="Wybrakowania [szt.]"
              type="number"
              InputProps={{ inputProps: { min: 0 } }}
              error={!!errors.defectiveCount}
              helperText={errors.defectiveCount?.message}
              {...register("defectiveCount", {
                required: "Liczba wybrakowań jest wymagana",
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
            Zapisz
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default AddProductionDataFailureModal;
