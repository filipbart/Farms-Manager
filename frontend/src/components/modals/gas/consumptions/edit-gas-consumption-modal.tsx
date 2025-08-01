import {
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  Button,
} from "@mui/material";
import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";

import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import AppDialog from "../../../common/app-dialog";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { GasService } from "../../../../services/gas-service";
import type {
  GasConsumptionListModel,
  UpdateGasConsumptionData,
} from "../../../../models/gas/gas-consumptions";

interface EditGasConsumptionModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  gasConsumption: GasConsumptionListModel | null;
}

const EditGasConsumptionModal: React.FC<EditGasConsumptionModalProps> = ({
  open,
  onClose,
  onSave,
  gasConsumption,
}) => {
  const [loading, setLoading] = useState(false);
  const [isCalculatingCost, setIsCalculatingCost] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<UpdateGasConsumptionData>();

  const quantityConsumed = watch("quantityConsumed");

  useEffect(() => {
    if (gasConsumption) {
      reset({
        quantityConsumed: gasConsumption.quantityConsumed,
        cost: gasConsumption.cost,
      });
    }
  }, [gasConsumption, reset]);

  useEffect(() => {
    const calculateCost = async () => {
      if (gasConsumption?.farmId && quantityConsumed > 0) {
        setIsCalculatingCost(true);
        try {
          const response = await GasService.calculateCost({
            farmId: gasConsumption.farmId,
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
  }, [quantityConsumed, gasConsumption, setValue]);

  const handleClose = () => {
    reset();
    onClose();
  };

  const handleSave = async (data: UpdateGasConsumptionData) => {
    if (!gasConsumption) return;
    setLoading(true);
    await handleApiResponse(
      () => GasService.updateGasConsumption(gasConsumption.id, data),
      () => {
        toast.success("Pomyślnie zaktualizowano wpis zużycia gazu");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji wpisu"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Edytuj wpis zużycia gazu</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2.5} sx={{ mt: 0.5 }}>
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Ferma"
                value={gasConsumption?.farmName || ""}
                InputProps={{ readOnly: true }}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <TextField
                label="Cykl"
                value={gasConsumption?.cycleText || ""}
                InputProps={{ readOnly: true }}
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
                InputProps={{ readOnly: true }}
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
            Zapisz zmiany
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default EditGasConsumptionModal;
