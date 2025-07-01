import {
  Dialog,
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
import {
  HatcheriesService,
  type AddHatcheryFormData,
} from "../../../services/hatcheries-service";
import { MdSave } from "react-icons/md";

interface AddHatcheryModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddHatcheryModal: React.FC<AddHatcheryModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<AddHatcheryFormData>();

  const [loading, setLoading] = useState(false);

  const handleSave = async (data: AddHatcheryFormData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => HatcheriesService.addHatcheryAsync(data),
      () => {
        toast.success("Wylęgarnia została dodana");
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania wylęgarni"
    );

    setLoading(false);
  };

  const close = () => {
    reset();
    onClose();
  };

  return (
    <Dialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Wprowadź dane nowej wylęgarni</DialogTitle>
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
              })}
              fullWidth
            />

            <TextField
              label="Pełna nazwa"
              error={!!errors?.fullName}
              helperText={
                errors.fullName ? (errors.fullName.message as string) : ""
              }
              {...register("fullName", {
                required: "Pełna nazwa jest wymagana",
              })}
              fullWidth
            />

            <TextField
              label="NIP"
              error={!!errors?.nip}
              helperText={errors.nip ? (errors.nip.message as string) : ""}
              {...register("nip", {
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
    </Dialog>
  );
};

export default AddHatcheryModal;
