import {
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
} from "@mui/material";
import { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import LoadingButton from "../../../common/loading-button";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { toast } from "react-toastify";
import type { FeedDeliveryListModel } from "../../../../models/feeds/deliveries/feed-invoice";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { FeedsService } from "../../../../services/feeds-service";
import { MdSave } from "react-icons/md";

interface EditInvoiceModalProps {
  open: boolean;
  onClose: () => void;
  feedDelivery: FeedDeliveryListModel | null;
  onSave: () => void;
}

const EditFeedDeliveryModal: React.FC<EditInvoiceModalProps> = ({
  open,
  onClose,
  feedDelivery,
  onSave,
}) => {
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
  } = useForm<FeedDeliveryListModel>({
    defaultValues: feedDelivery!,
  });

  useEffect(() => {
    if (feedDelivery) {
      reset(feedDelivery);
    }
  }, [feedDelivery, reset]);

  const handleSave = async (formData: FeedDeliveryListModel) => {
    setLoading(true);

    await handleApiResponse(
      () => FeedsService.updateFeedDelivery(feedDelivery!.id, formData),
      () => {
        toast.success(
          `Pomyślnie zapisano zmiany faktury: ${formData.invoiceNumber}`
        );
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania zmian faktury"
    );

    setLoading(false);
  };

  return (
    <Dialog
      open={open}
      onClose={(_event, reason) => {
        if (reason !== "backdropClick") {
          onClose();
        }
      }}
      fullWidth
      maxWidth="md"
    >
      <DialogTitle>Edycja faktury</DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent dividers>
          <Grid container spacing={2}>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Numer faktury"
                error={!!errors.invoiceNumber}
                helperText={errors.invoiceNumber?.message}
                {...register("invoiceNumber", {
                  required: "Numer faktury jest wymagany",
                })}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Numer konta bankowego"
                error={!!errors.bankAccountNumber}
                helperText={errors.bankAccountNumber?.message}
                {...register("bankAccountNumber", {
                  required: "Numer konta bankowego jest wymagany",
                })}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Nazwa paszy"
                error={!!errors.itemName}
                helperText={errors.itemName?.message}
                {...register("itemName", {
                  required: "Nazwa paszy jest wymagana",
                })}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Sprzedawca"
                error={!!errors.vendorName}
                helperText={errors.vendorName?.message}
                {...register("vendorName", {
                  required: "Sprzedawca jest wymagany",
                })}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Ilość"
                type="number"
                inputMode="decimal"
                slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                error={!!errors.quantity}
                helperText={errors.quantity?.message}
                {...register("quantity", {
                  required: "Ilość jest wymagana",
                  valueAsNumber: true,
                })}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Cena jednostkowa [zł]"
                type="number"
                inputMode="decimal"
                slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                error={!!errors.unitPrice}
                helperText={errors.unitPrice?.message}
                {...register("unitPrice", {
                  required: "Cena jednostkowa jest wymagana",
                  valueAsNumber: true,
                })}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <Controller
                name="invoiceDate"
                control={control}
                rules={{
                  required: "Data wystawienia faktury jest wymagana",
                }}
                render={({ field }) => (
                  <DatePicker
                    label="Data wystawienia faktury"
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
              <Controller
                name="dueDate"
                control={control}
                rules={{
                  required: "Termin płatności faktury jest wymagany",
                }}
                render={({ field }) => (
                  <DatePicker
                    label="Termin płatności"
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
                        error: !!errors.dueDate,
                        helperText: errors.dueDate?.message,
                      },
                    }}
                  />
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                label="Wartość brutto [zł]"
                type="number"
                inputMode="decimal"
                slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                error={!!errors.invoiceTotal}
                helperText={errors.invoiceTotal?.message}
                {...register("invoiceTotal", {
                  required: "Wartość brutto jest wymagana",
                  valueAsNumber: true,
                })}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                label="Wartość netto [zł]"
                type="number"
                inputMode="decimal"
                slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                error={!!errors.subTotal}
                helperText={errors.subTotal?.message}
                {...register("subTotal", {
                  required: "Wartość netto jest wymagana",
                  valueAsNumber: true,
                })}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                label="VAT [zł]"
                type="number"
                inputMode="decimal"
                slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                error={!!errors.vatAmount}
                helperText={errors.vatAmount?.message}
                {...register("vatAmount", {
                  required: "VAT jest wymagany",
                  valueAsNumber: true,
                })}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <TextField
                label="Notatka"
                error={!!errors.comment}
                helperText={errors.comment?.message}
                {...register("comment")}
                fullWidth
                multiline
                rows={2}
              />
            </Grid>
          </Grid>
        </DialogContent>

        <DialogActions>
          <Button onClick={onClose} color="secondary" variant="outlined">
            Anuluj
          </Button>
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
    </Dialog>
  );
};

export default EditFeedDeliveryModal;
