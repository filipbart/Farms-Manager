import {
  Typography,
  Button,
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  MenuItem,
  Box,
  Autocomplete,
} from "@mui/material";
import { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave, MdAttachFile } from "react-icons/md";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";

import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import FilePreview from "../../../common/file-preview";
import { useFarms } from "../../../../hooks/useFarms";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import type { AddExpenseProductionData } from "../../../../models/expenses/production/expenses-productions";
import { useExpensesContractor } from "../../../../hooks/expenses/useExpensesContractors";
import { ExpensesService } from "../../../../services/expenses-service";
import AppDialog from "../../../common/app-dialog";

interface AddExpenseProductionModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddExpenseProductionModal: React.FC<AddExpenseProductionModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const {
    expensesContractors,
    loadingExpensesContractors,
    fetchExpensesContractors,
  } = useExpensesContractor();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();

  const [selectedFile, setSelectedFile] = useState<File>();
  const [loading, setLoading] = useState(false);

  const filePreviewUrl = selectedFile ? URL.createObjectURL(selectedFile) : "";

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    clearErrors,
    setError,
    watch,
  } = useForm<AddExpenseProductionData>({
    defaultValues: {
      farmId: "",
      cycleId: "",
      cycleDisplay: "",
      expenseContractorId: "",
      expenseTypeNameDisplay: "",
      invoiceNumber: "",
      invoiceTotal: 0,
      subTotal: 0,
      vatAmount: 0,
      invoiceDate: "",
      comment: "",
    },
  });

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchExpensesContractors();
    }
  }, [open, fetchFarms, fetchExpensesContractors]);

  useEffect(() => {
    return () => {
      if (filePreviewUrl) {
        URL.revokeObjectURL(filePreviewUrl);
      }
    };
  }, [filePreviewUrl]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.[0]) {
      const file = e.target.files[0];
      setSelectedFile(file);
      setValue("file", file);
    }
  };

  const handleFarmChange = async (farmId: string) => {
    setValue("cycleId", "");
    setValue("cycleDisplay", "");
    clearErrors("cycleId");

    const cycle = await loadLatestCycle(farmId);
    if (cycle) {
      setValue("cycleId", cycle.id);
      setValue("cycleDisplay", `${cycle.identifier}/${cycle.year}`);
      clearErrors("cycleId");
    } else {
      setError("cycleId", {
        type: "manual",
        message: "Brak aktywnego cyklu dla wybranej fermy",
      });
    }
  };

  const handleContractorChange = (contractorId: string) => {
    const selected = expensesContractors.find((c) => c.id === contractorId);
    if (selected) {
      setValue("expenseTypeNameDisplay", selected.expenseType || "Brak");
    } else {
      setValue("expenseTypeNameDisplay", "");
    }
  };

  const handleClose = () => {
    setSelectedFile(undefined);
    reset();
    onClose();
  };

  const handleSave = async (data: AddExpenseProductionData) => {
    setLoading(true);
    await handleApiResponse(
      () => ExpensesService.addExpenseProduction(data),
      () => {
        toast.success("Pomyślnie dodano koszt produkcji");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas dodawania kosztu produkcji"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="lg" fullWidth>
      <DialogTitle>Dodaj koszt produkcji</DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, md: 5 }}>
              <Box display="flex" flexDirection="column" gap={2}>
                <Button
                  variant="outlined"
                  component="label"
                  startIcon={<MdAttachFile />}
                  fullWidth
                >
                  Wybierz plik faktury
                  <input type="file" hidden onChange={handleFileChange} />
                </Button>
                {selectedFile && (
                  <>
                    <Typography variant="body2" noWrap>
                      Wybrano plik: {selectedFile.name}
                    </Typography>
                    <Box mt={1}>
                      <FilePreview file={selectedFile} maxHeight={700} />
                    </Box>
                  </>
                )}
              </Box>
            </Grid>

            <Grid size={{ xs: 12, md: 7 }}>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <LoadingTextField
                    label="Ferma"
                    select
                    fullWidth
                    loading={loadingFarms}
                    value={watch("farmId") || ""}
                    error={!!errors.farmId}
                    helperText={errors.farmId?.message}
                    {...register("farmId", {
                      required: "Farma jest wymagana",
                      onChange: (e) => handleFarmChange(e.target.value),
                    })}
                  >
                    {farms.map((farm) => (
                      <MenuItem key={farm.id} value={farm.id}>
                        {farm.name}
                      </MenuItem>
                    ))}
                  </LoadingTextField>
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <LoadingTextField
                    loading={loadingCycle}
                    label="Cykl"
                    value={watch("cycleDisplay") || ""}
                    slotProps={{ input: { readOnly: true } }}
                    error={!!errors.cycleId}
                    helperText={errors.cycleId?.message}
                    fullWidth
                  />
                </Grid>

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

                <Grid size={{ xs: 12, sm: 6 }}>
                  <Controller
                    name="expenseContractorId"
                    control={control}
                    rules={{ required: "Kontrahent jest wymagany" }}
                    render={({ field, fieldState: { error } }) => (
                      <Autocomplete
                        options={expensesContractors}
                        getOptionLabel={(option) => option.name || ""}
                        isOptionEqualToValue={(option, value) =>
                          option.id === value.id
                        }
                        loading={loadingExpensesContractors}
                        value={
                          expensesContractors.find(
                            (contractor) => contractor.id === field.value
                          ) || null
                        }
                        onChange={(_, newValue) => {
                          const newId = newValue ? newValue.id : "";
                          field.onChange(newId);
                          handleContractorChange(newId);
                        }}
                        renderInput={(params) => (
                          <TextField
                            {...params}
                            label="Kontrahent"
                            error={!!error}
                            helperText={error?.message}
                          />
                        )}
                      />
                    )}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Typ wydatku"
                    value={watch("expenseTypeNameDisplay") || ""}
                    slotProps={{ input: { readOnly: true } }}
                    fullWidth
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

                <Grid size={{ xs: 12, sm: 4 }}>
                  <TextField
                    label="Netto [zł]"
                    type="number"
                    slotProps={{ htmlInput: { step: "0.01", min: 0 } }}
                    {...register("subTotal", {
                      required: "Wartość netto jest wymagana",
                      valueAsNumber: true,
                      validate: (value) =>
                        value >= 0 || "Wartość netto nie może być ujemna",
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
                    slotProps={{ htmlInput: { step: "0.01", min: 0 } }}
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
                    slotProps={{ htmlInput: { step: "0.01", min: 0 } }}
                    {...register("invoiceTotal", {
                      required: "Wartość brutto jest wymagana",
                      valueAsNumber: true,
                      validate: (value) =>
                        value >= 0 || "Wartość brutto nie może być ujemna",
                    })}
                    error={!!errors.invoiceTotal}
                    helperText={errors.invoiceTotal?.message}
                    fullWidth
                  />
                </Grid>
              </Grid>
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
            Zapisz
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default AddExpenseProductionModal;
