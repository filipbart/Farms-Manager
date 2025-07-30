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
import { toast } from "react-toastify";
import { useFarms } from "../../../../hooks/useFarms";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { useHatcheries } from "../../../../hooks/useHatcheries";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import AppDialog from "../../../common/app-dialog";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import type { AddWeighingData } from "../../../../models/production-data/weighings";
import { ProductionDataWeighingsService } from "../../../../services/production-data/production-data-weighings-service";

interface AddProductionDataWeighingModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddProductionDataWeighingModal: React.FC<
  AddProductionDataWeighingModalProps
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
  } = useForm<AddWeighingData & { cycleDisplay: string }>();

  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { hatcheries, loadingHatcheries, fetchHatcheries } = useHatcheries();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();
  const [availableHenhouses, setAvailableHenhouses] = useState<HouseRowModel[]>(
    []
  );

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchHatcheries();
    }
  }, [open, fetchFarms, fetchHatcheries]);

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

  const handleSave = async (data: AddWeighingData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => ProductionDataWeighingsService.addWeighing(data),
      () => {
        toast.success("Pomyślnie dodano wpis ważenia");
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
      <DialogTitle>Dodaj nowy wpis ważenia</DialogTitle>
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
              InputProps={{ readOnly: true }}
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

            <LoadingTextField
              loading={loadingHatcheries}
              select
              label="Wylęgarnia"
              fullWidth
              error={!!errors.hatcheryId}
              helperText={errors.hatcheryId?.message}
              {...register("hatcheryId", {
                required: "Wylęgarnia jest wymagana",
              })}
            >
              {hatcheries.map((hatchery) => (
                <MenuItem key={hatchery.id} value={hatchery.id}>
                  {hatchery.name}
                </MenuItem>
              ))}
            </LoadingTextField>

            <TextField
              label="Doba ważenia"
              type="number"
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
              label="Średnia masa ciała [g]"
              type="number"
              InputProps={{ inputProps: { min: 0 } }}
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
            Zapisz
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default AddProductionDataWeighingModal;
