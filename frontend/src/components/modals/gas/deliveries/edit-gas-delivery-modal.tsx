import {
  Button,
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
} from "@mui/material";
import { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import LoadingButton from "../../../common/loading-button";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import AppDialog from "../../../common/app-dialog";
import type {
  GasDeliveryListModel,
  UpdateGasDeliveryData,
} from "../../../../models/gas/gas-deliveries";
import { GasService } from "../../../../services/gas-service";

interface EditGasDeliveryModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  gasDelivery: GasDeliveryListModel | null;
}

const EditGasDeliveryModal: React.FC<EditGasDeliveryModalProps> = ({
  open,
  onClose,
  onSave,
  gasDelivery,
}) => {
  const [loading, setLoading] = useState(false);

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm<UpdateGasDeliveryData>();

  useEffect(() => {
    if (gasDelivery) {
      reset({
        invoiceNumber: gasDelivery.invoiceNumber,
        invoiceDate: gasDelivery.invoiceDate,
        unitPrice: gasDelivery.unitPrice,
        quantity: gasDelivery.quantity,
        comment: gasDelivery.comment,
      });
    }
  }, [gasDelivery, reset]);

  const handleClose = () => {
    reset();
    onClose();
  };

  const handleSave = async (data: UpdateGasDeliveryData) => {
    if (!gasDelivery) return;

    setLoading(true);
    await handleApiResponse(
      () => GasService.updateGasDelivery(gasDelivery.id, data),
      () => {
        toast.success("Pomyślnie zaktualizowano dostawę gazu");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji dostawy gazu"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle>Edytuj dostawę gazu</DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Ferma"
                value={gasDelivery?.farmName || ""}
                InputProps={{ readOnly: true }}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Kontrahent"
                value={gasDelivery?.contractorName || ""}
                InputProps={{ readOnly: true }}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Numer faktury"
                value={watch("invoiceNumber") || ""}
                {...register("invoiceNumber", {
                  required: "Numer faktury jest wymagany",
                })}
                error={!!errors.invoiceNumber}
                helperText={errors.invoiceNumber?.message}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <Controller
                name="invoiceDate"
                control={control}
                rules={{ required: "Data faktury jest wymagana" }}
                render={({ field }) => (
                  <DatePicker
                    label="Data faktury"
                    format="DD.MM.YYYY"
                    value={field.value ? dayjs(field.value) : null}
                    onChange={(date) =>
                      field.onChange(
                        date ? dayjs(date).format("YYYY-MM-DD") : ""
                      )
                    }
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        error: !!errors.invoiceDate,
                        helperText: errors.invoiceDate?.message,
                      },
                    }}
                  />
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Cena jednostkowa [zł]"
                type="number"
                value={watch("unitPrice") || ""}
                slotProps={{ htmlInput: { step: "any" } }}
                {...register("unitPrice", {
                  required: "Cena jest wymagana",
                  valueAsNumber: true,
                  validate: (value) =>
                    value > 0 || "Cena musi być większa od 0",
                })}
                error={!!errors.unitPrice}
                helperText={errors.unitPrice?.message}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Ilość [l]"
                type="number"
                value={watch("quantity") || ""}
                slotProps={{ htmlInput: { step: "any" } }}
                {...register("quantity", {
                  required: "Ilość jest wymagana",
                  valueAsNumber: true,
                  validate: (value) =>
                    value > 0 || "Ilość musi być większa od 0",
                })}
                error={!!errors.quantity}
                helperText={errors.quantity?.message}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <TextField
                label="Komentarz"
                value={watch("comment") || ""}
                {...register("comment")}
                multiline
                rows={4}
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
            loading={loading}
          >
            Zapisz zmiany
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default EditGasDeliveryModal;
