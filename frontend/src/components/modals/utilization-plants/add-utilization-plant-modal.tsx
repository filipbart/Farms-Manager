import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
} from "@mui/material";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import LoadingButton from "../../common/loading-button";
import { MdSave } from "react-icons/md";
import { UtilizationPlantsService } from "../../../services/utilization-plants-service";
import { isValidNip, isValidProducerNumber } from "../../../utils/validation";
import AppDialog from "../../common/app-dialog";
import type { AddUtilizationPlantFormData } from "../../../models/utilization-plants/utilization-plants";

interface AddUtilizationPlantModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddUtilizationPlantModal: React.FC<AddUtilizationPlantModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<AddUtilizationPlantFormData>();

  const [loading, setLoading] = useState(false);

  const handleSave = async (data: AddUtilizationPlantFormData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => UtilizationPlantsService.addUtilizationPlantAsync(data),
      () => {
        toast.success("Zakład utylizacyjny został dodany");
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania zakładu utylizacyjnego"
    );

    setLoading(false);
  };

  const close = () => {
    reset();
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Wprowadź dane nowego zakładu utylizacyjnego</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <TextField
              label="Nazwa"
              error={!!errors?.name}
              helperText={errors.name ? (errors.name.message as string) : ""}
              {...register("name", {
                required: "Nazwa jest wymagana",
              })}
              fullWidth
            />

            <TextField
              label="Numer IRZplus"
              error={!!errors?.irzNumber}
              helperText={
                errors.irzNumber ? (errors.irzNumber.message as string) : ""
              }
              {...register("irzNumber", {
                required: "Numer IRZplus jest wymagany",
                validate: (value) =>
                  isValidProducerNumber(value) || "Numer IRZplus musi mieć format: liczba-liczba (np. 000111222-012)",
              })}
              fullWidth
            />

            <TextField
              label="NIP"
              error={!!errors?.nip}
              helperText={errors.nip ? (errors.nip.message as string) : ""}
              {...register("nip", {
                validate: (value) =>
                  isValidNip(value) || "NIP jest nieprawidłowy",
                required: "NIP jest wymagany",
              })}
              fullWidth
            />
            <TextField
              label="Adres"
              error={!!errors?.address}
              helperText={
                errors.address ? (errors.address.message as string) : ""
              }
              {...register("address")}
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
            startIcon={<MdSave />}
            type="submit"
            variant="contained"
            color="primary"
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

export default AddUtilizationPlantModal;
