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
import {
  type AddFarmFormData,
  FarmsService,
} from "../../../services/farms-service";
import { toast } from "react-toastify";

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

    try {
      const response = await FarmsService.addFarmAsync(data);
      if (response.success) {
        toast.success("Farma została dodana");
        onSave();
      } else {
        if (response.domainException?.errorDescription) {
          toast.error(response.domainException.errorDescription);
        } else if (response.errors && typeof response.errors === "object") {
          Object.values(response.errors).forEach((err: any) => {
            toast.error(err);
          });
        } else {
          toast.error("Nie udało się zapisać farmy");
        }
      }
    } catch (e) {
      toast.error("Wystąpił błąd podczas zapisywania farmy.");
    } finally {
      setLoading(false);
    }
  };

  const close = () => {
    reset();
    onClose();
  };

  return (
    <Dialog open={open} onClose={close} fullWidth maxWidth="sm">
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
              {...register("address", {
                required: "Adres jest wymagany",
              })}
              fullWidth
            />
          </Box>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={close} variant="outlined" color="inherit">
            Anuluj
          </Button>
          <Button type="submit" variant="contained" color="primary">
            Zapisz
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default AddFarmModal;
