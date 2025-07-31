import {
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  MenuItem,
  Button,
} from "@mui/material";
import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";

import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import AppDialog from "../../../common/app-dialog";
import { useFarms } from "../../../../hooks/useFarms";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import type { AddGasConsumptionData } from "../../../../models/gas/gas-consumptions";

interface AddGasConsumptionModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddGasConsumptionModal: React.FC<AddGasConsumptionModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();
  const [loading, setLoading] = useState(false);
  const [isCalculatingCost, setIsCalculatingCost] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    setError,
    clearErrors,
    watch,
  } = useForm<AddGasConsumptionData & { cycleDisplay: string }>();

  const farmId = watch("farmId");
  const quantityConsumed = watch("quantityConsumed");

  useEffect(() => {
    if (open) {
      fetchFarms();
    }
  }, [open, fetchFarms]);

  useEffect(() => {
    const calculateCost = async () => {
      if (farmId && quantityConsumed > 0) {
        setIsCalculatingCost(true);
        try {
          const response = await GasConsumptionsService.calculateCost({
            farmId,
            quantity: quantityConsumed,
          });
          if (response.success && response.responseData) {
            setValue("cost", response.responseData.cost);
          }
        } catch (error) {
          console.error("Błąd podczas obliczania kosztu", error);
          setValue("cost", 0);
        } finally {
          setIsCalculatingCost(false);
        }
      }
    };

    const debounceTimeout = setTimeout(() => {
      calculateCost();
    }, 500);

    return () => clearTimeout(debounceTimeout);
  }, [farmId, quantityConsumed, setValue]);

  const handleFarmChange = async (farmId: string) => {
    setValue("cycleId", "");
    setValue("cycleDisplay", "");
    clearErrors("cycleId");

    const cycle = await loadLatestCycle(farmId);
    if (cycle) {
      setValue("cycleId", cycle.id);
      setValue("cycleDisplay", `${cycle.identifier}/${cycle.year}`);
      clearErrors("cycleId");
    } else {
      setError("cycleId", {
        type: "manual",
        message: "Brak aktywnego cyklu dla wybranej fermy",
      });
    }
  };

  const handleClose = () => {
    reset();
    onClose();
  };

  const handleSave = async (data: AddGasConsumptionData) => {
    setLoading(true);
    await handleApiResponse(
      () => GasConsumptionsService.addGasConsumption(data),
      () => {
        toast.success("Pomyślnie dodano wpis zużycia gazu");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas dodawania wpisu"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Dodaj wpis zużycia gazu</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2.5} sx={{ mt: 0.5 }}>
            <Grid size={{ xs: 12 }}>
              <LoadingTextField
                label="Ferma"
                select
                fullWidth
                loading={loadingFarms}
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
            </Grid>

            <Grid size={{ xs: 12 }}>
              <LoadingTextField
                loading={loadingCycle}
                label="Cykl"
                value={watch("cycleDisplay") || ""}
                slotProps={{ input: { readOnly: true } }}
                error={!!errors.cycleId}
                helperText={errors.cycleId?.message}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Ilość zużytego gazu [l]"
                type="number"
                slotProps={{ htmlInput: { step: "any" } }}
                {...register("quantityConsumed", {
                  required: "Ilość jest wymagana",
                  valueAsNumber: true,
                  validate: (value) =>
                    value > 0 || "Ilość musi być większa od 0",
                })}
                error={!!errors.quantityConsumed}
                helperText={errors.quantityConsumed?.message}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <LoadingTextField
                label="Koszt gazu [zł]"
                type="number"
                loading={isCalculatingCost}
                value={String(watch("cost") ?? "")}
                slotProps={{ input: { readOnly: true } }}
                error={!!errors.cost}
                helperText={errors.cost?.message}
                {...register("cost", {
                  required: "Koszt jest wymagany",
                  valueAsNumber: true,
                  validate: (value) =>
                    value > 0 || "Koszt musi być większy od 0",
                })}
                fullWidth
              />
            </Grid>
          </Grid>
        </DialogContent>

        <DialogActions>
          <Button onClick={handleClose}>Anuluj</Button>
          <LoadingButton
            type="submit"
            variant="contained"
            color="primary"
            startIcon={<MdSave />}
            disabled={loading || isCalculatingCost}
            loading={loading}
          >
            Zapisz
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default AddGasConsumptionModal;
