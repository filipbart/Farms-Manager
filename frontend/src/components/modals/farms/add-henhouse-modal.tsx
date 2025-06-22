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
  type AddHenhouseFormData,
  FarmsService,
} from "../../../services/farms-service";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import LoadingButton from "../../common/loading-button";

interface AddHenhouseModalProps {
  farmId: string;
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddHenhouseModal: React.FC<AddHenhouseModalProps> = ({
  farmId,
  open,
  onClose,
  onSave,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<AddHenhouseFormData>();

  const [loading, setLoading] = useState(false);

  const handleSave = async (data: AddHenhouseFormData) => {
    data.farmId = farmId;
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => FarmsService.addHenhouseAsync(data),
      () => {
        toast.success("Kurnik został dodany");
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas dodawania kurnika"
    );

    setLoading(false);
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
              type="number"
              label="Powierzchnia (m²)"
              error={!!errors?.area}
              helperText={errors.area ? (errors.area.message as string) : ""}
              {...register("area", {
                required: "Powierzchnia jest wymagana",
              })}
              fullWidth
            />
            <TextField
              label="Opis"
              error={!!errors?.desc}
              helperText={errors.desc ? (errors.desc.message as string) : ""}
              {...register("desc")}
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

export default AddHenhouseModal;
