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
import { ExpensesService } from "../../../../services/expenses-service";
import type {
  ExpenseProductionListModel,
  UpdateExpenseProductionData,
} from "../../../../models/expenses/production/expenses-productions";
import AppDialog from "../../../common/app-dialog";

interface EditExpenseProductionModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  expenseProductionToEdit: ExpenseProductionListModel | null;
}

const EditExpenseProductionModal: React.FC<EditExpenseProductionModalProps> = ({
  open,
  onClose,
  onSave,
  expenseProductionToEdit,
}) => {
  const [loading, setLoading] = useState(false);

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<UpdateExpenseProductionData>();

  useEffect(() => {
    if (expenseProductionToEdit) {
      reset({
        invoiceNumber: expenseProductionToEdit.invoiceNumber,
        invoiceTotal: expenseProductionToEdit.invoiceTotal,
        subTotal: expenseProductionToEdit.subTotal,
        vatAmount: expenseProductionToEdit.vatAmount,
        invoiceDate: expenseProductionToEdit.invoiceDate,
      });
    }
  }, [expenseProductionToEdit, reset]);

  const handleClose = () => {
    reset();
    onClose();
  };

  const handleSave = async (data: UpdateExpenseProductionData) => {
    if (!expenseProductionToEdit) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        ExpensesService.updateExpenseProduction(
          expenseProductionToEdit.id,
          data
        ),
      () => {
        toast.success("Pomyślnie zaktualizowano koszt produkcji");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji kosztu produkcji"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle>Edytuj koszt produkcji</DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Numer faktury"
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

            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                label="Netto [zł]"
                type="number"
                InputProps={{ slotProps: { input: { step: "any" } } }}
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
                InputProps={{ slotProps: { input: { step: "any" } } }}
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
                InputProps={{ slotProps: { input: { step: "any" } } }}
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

export default EditExpenseProductionModal;
