import {
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  MenuItem,
  Button,
  Box,
  Typography,
} from "@mui/material";
import { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave, MdAttachFile } from "react-icons/md";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import LoadingButton from "../../../common/loading-button";
import AppDialog from "../../../common/app-dialog";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { useAdvanceCategories } from "../../../../hooks/expenses/advances/useAdvanceCategories";
import { AdvanceType } from "../../../../models/expenses/advances/categories";
import type {
  ExpenseAdvanceListModel,
  UpdateExpenseAdvance,
} from "../../../../models/expenses/advances/expenses-advances";
import { ExpensesAdvancesService } from "../../../../services/expenses-advances-service";

interface EditExpenseAdvanceModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  advance: ExpenseAdvanceListModel | null;
}

const EditExpenseAdvanceModal: React.FC<EditExpenseAdvanceModalProps> = ({
  open,
  onClose,
  onSave,
  advance,
}) => {
  const [loading, setLoading] = useState(false);
  const { categories, fetchCategories } = useAdvanceCategories();
  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
    setValue,
  } = useForm<UpdateExpenseAdvance>();

  const selectedType = watch("type");

  useEffect(() => {
    if (open) {
      fetchCategories();
    }
  }, [open, fetchCategories]);

  useEffect(() => {
    if (advance) {
      reset({
        date: advance.date,
        type: advance.type,
        name: advance.name,
        amount: advance.amount,
        categoryName: advance.categoryName,
        comment: advance.comment,
      });
      setSelectedFile(null); // Reset pliku przy otwarciu
    }
  }, [advance, reset]);

  useEffect(() => {
    setValue("file", selectedFile || undefined);
  }, [selectedFile, setValue]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.[0]) {
      setSelectedFile(e.target.files[0]);
    }
  };

  const handleClose = () => {
    reset();
    setSelectedFile(null);
    onClose();
  };

  const handleSave = async (data: UpdateExpenseAdvance) => {
    if (!advance) return;
    setLoading(true);
    await handleApiResponse(
      () => ExpensesAdvancesService.updateExpenseAdvance(advance.id, data),
      () => {
        toast.success("Pomyślnie zaktualizowano wpis");
        handleClose();
        onSave();
      },
      undefined,
      "Błąd podczas aktualizacji wpisu"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="md">
      <DialogTitle>Edytuj wpis zaliczki</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2.5} sx={{ mt: 0.5 }}>
            <Grid size={{ xs: 12, sm: 6 }}>
              <Controller
                name="date"
                control={control}
                rules={{ required: "Data jest wymagana" }}
                render={({ field }) => (
                  <DatePicker
                    label="Data"
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
                        error: !!errors.date,
                        helperText: errors.date?.message,
                      },
                    }}
                  />
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                select
                label="Typ"
                fullWidth
                value={watch("type") || ""}
                error={!!errors.type}
                helperText={errors.type?.message}
                {...register("type", { required: "Typ jest wymagany" })}
              >
                <MenuItem value={AdvanceType.Expense}>
                  Wydatek (zaliczka)
                </MenuItem>
                <MenuItem value={AdvanceType.Income}>Przychód (zwrot)</MenuItem>
              </TextField>
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                select
                label="Kategoria"
                fullWidth
                value={watch("categoryName") || ""}
                error={!!errors.categoryName}
                helperText={errors.categoryName?.message}
                {...register("categoryName", {
                  required: "Kategoria jest wymagana",
                })}
              >
                {categories
                  .filter((c) => c.type === selectedType)
                  .map((cat) => (
                    <MenuItem key={cat.id} value={cat.name}>
                      {cat.name}
                    </MenuItem>
                  ))}
              </TextField>
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Nazwa"
                fullWidth
                error={!!errors.name}
                helperText={errors.name?.message}
                {...register("name", { required: "Nazwa jest wymagana" })}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Kwota [zł]"
                type="number"
                fullWidth
                slotProps={{ htmlInput: { step: "0.01", min: 0 } }}
                error={!!errors.amount}
                helperText={errors.amount?.message}
                {...register("amount", {
                  required: "Kwota jest wymagana",
                  valueAsNumber: true,
                  min: { value: 0.01, message: "Kwota musi być większa od 0" },
                })}
              />
            </Grid>
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Komentarz"
                multiline
                rows={3}
                fullWidth
                {...register("comment")}
              />
            </Grid>
            <Grid size={{ xs: 12 }}>
              <Box display="flex" alignItems="center" gap={2}>
                <Button
                  component="label"
                  variant="outlined"
                  startIcon={<MdAttachFile />}
                >
                  Załącz plik
                  <input type="file" hidden onChange={handleFileChange} />
                </Button>
                {selectedFile && (
                  <Typography variant="body2">{selectedFile.name}</Typography>
                )}
              </Box>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} variant="outlined" color="inherit">
            Anuluj
          </Button>
          <LoadingButton
            type="submit"
            variant="contained"
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

export default EditExpenseAdvanceModal;
