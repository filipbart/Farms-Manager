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
  SalesInvoiceListModel,
  UpdateSalesInvoiceData,
} from "../../../../models/sales/sales-invoices";
import { SalesService } from "../../../../services/sales-service";

interface EditSalesInvoiceModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  salesInvoice: SalesInvoiceListModel | null;
}

const EditSaleInvoiceModal: React.FC<EditSalesInvoiceModalProps> = ({
  open,
  onClose,
  onSave,
  salesInvoice,
}) => {
  const [loading, setLoading] = useState(false);

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm<UpdateSalesInvoiceData>();

  useEffect(() => {
    if (salesInvoice) {
      reset({
        invoiceNumber: salesInvoice.invoiceNumber,
        invoiceTotal: salesInvoice.invoiceTotal,
        subTotal: salesInvoice.subTotal,
        vatAmount: salesInvoice.vatAmount,
        invoiceDate: salesInvoice.invoiceDate,
        dueDate: salesInvoice.dueDate,
        paymentDate: salesInvoice.paymentDate,
      });
    }
  }, [salesInvoice, reset]);

  const handleClose = () => {
    reset();
    onClose();
  };

  const handleSave = async (data: UpdateSalesInvoiceData) => {
    if (!salesInvoice) return;

    setLoading(true);
    await handleApiResponse(
      () => SalesService.updateSaleInvoice(salesInvoice.id, data),
      () => {
        toast.success("Pomyślnie zaktualizowano fakturę sprzedaży");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji faktury"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle>Edytuj fakturę sprzedaży</DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
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
              <Controller
                name="dueDate"
                control={control}
                rules={{ required: "Termin płatności jest wymagany" }}
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
            <Grid size={{ xs: 12, sm: 6 }}>
              <Controller
                name="paymentDate"
                control={control}
                render={({ field }) => (
                  <DatePicker
                    label="Data płatności (opcjonalnie)"
                    format="DD.MM.YYYY"
                    value={field.value ? dayjs(field.value) : null}
                    onChange={(date) =>
                      field.onChange(
                        date ? dayjs(date).format("YYYY-MM-DD") : null
                      )
                    }
                    slotProps={{
                      textField: {
                        fullWidth: true,
                      },

                      actionBar: {
                        actions: ["clear", "cancel", "accept"],
                      },
                    }}
                  />
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                label="Netto [zł]"
                type="number"
                value={watch("subTotal") || ""}
                slotProps={{ htmlInput: { step: "any" } }}
                {...register("subTotal", {
                  required: "Wartość netto jest wymagana",
                  valueAsNumber: true,
                  validate: (value) =>
                    value > 0 || "Wartość netto musi być większa od 0",
                })}
                error={!!errors.subTotal}
                helperText={errors.subTotal?.message}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                label="VAT [zł]"
                type="number"
                value={watch("vatAmount") || ""}
                slotProps={{ htmlInput: { step: "any" } }}
                {...register("vatAmount", {
                  required: "VAT jest wymagany",
                  valueAsNumber: true,
                  validate: (value) =>
                    value >= 0 || "Wartość VAT nie może być ujemna",
                })}
                error={!!errors.vatAmount}
                helperText={errors.vatAmount?.message}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                label="Brutto [zł]"
                type="number"
                value={watch("invoiceTotal") || ""}
                slotProps={{ htmlInput: { step: "any" } }}
                {...register("invoiceTotal", {
                  required: "Wartość brutto jest wymagana",
                  valueAsNumber: true,
                  validate: (value) =>
                    value > 0 || "Wartość brutto musi być większa od 0",
                })}
                error={!!errors.invoiceTotal}
                helperText={errors.invoiceTotal?.message}
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

export default EditSaleInvoiceModal;
