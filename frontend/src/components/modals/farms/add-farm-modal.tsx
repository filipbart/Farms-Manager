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
import {
  type AddFarmFormData,
  FarmsService,
} from "../../../services/farms-service";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import LoadingButton from "../../common/loading-button";
import { MdSave } from "react-icons/md";
import AppDialog from "../../common/app-dialog";
import { isValidNip, isValidProducerNumber } from "../../../utils/validation";

interface AddFarmModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddFarmModal: React.FC<AddFarmModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<AddFarmFormData>();

  const [loading, setLoading] = useState(false);

  const handleSave = async (data: AddFarmFormData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => FarmsService.addFarmAsync(data),
      () => {
        toast.success("Farma została dodana");
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania farm"
    );

    setLoading(false);
  };

  const close = () => {
    reset();
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Wprowadź dane nowej fermy</DialogTitle>
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
              label="Numer producenta"
              error={!!errors?.prodNumber}
              helperText={
                errors.prodNumber ? (errors.prodNumber.message as string) : ""
              }
              {...register("prodNumber", {
                required: "Numer producenta jest wymagany",
                validate: (value) =>
                  isValidProducerNumber(value) || "Numer producenta musi mieć format: liczba-liczba (np. 000111222-012)",
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
              {...register("address", {
                required: "Adres jest wymagany",
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

export default AddFarmModal;
