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
import AppDialog from "../../common/app-dialog";
import { isValidNip } from "../../../utils/validation";
import type { AddTaxBusinessEntityFormData } from "../../../models/data/tax-business-entity";
import { TaxBusinessEntitiesService } from "../../../services/tax-business-entities-service";

interface AddTaxBusinessEntityModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddTaxBusinessEntityModal: React.FC<AddTaxBusinessEntityModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<AddTaxBusinessEntityFormData>();

  const [loading, setLoading] = useState(false);

  const handleSave = async (data: AddTaxBusinessEntityFormData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => TaxBusinessEntitiesService.addAsync(data),
      () => {
        toast.success("Podmiot został dodany");
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania podmiotu"
    );

    setLoading(false);
  };

  const close = () => {
    reset();
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Wprowadź dane nowego podmiotu</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <TextField
              label="NIP"
              error={!!errors?.nip}
              helperText={errors.nip ? (errors.nip.message as string) : ""}
              {...register("nip", {
                required: "NIP jest wymagany",
                validate: (value) =>
                  isValidNip(value) || "NIP jest nieprawidłowy",
              })}
              fullWidth
            />

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
              label="Typ działalności"
              error={!!errors?.businessType}
              helperText={
                errors.businessType
                  ? (errors.businessType.message as string)
                  : ""
              }
              {...register("businessType", {
                required: "Typ działalności jest wymagany",
              })}
              fullWidth
              placeholder="np. Ferma Drobiu, Gospodarstwo Rolne, Najem"
            />

            <TextField
              label="Opis (opcjonalnie)"
              {...register("description")}
              fullWidth
              multiline
              rows={3}
            />

            <TextField
              label="Token KSeF (opcjonalnie)"
              {...register("kSeFToken")}
              fullWidth
              type="password"
              placeholder="Token autoryzacyjny KSeF"
              helperText="Token zostanie zaszyfrowany przed zapisem"
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

export default AddTaxBusinessEntityModal;
