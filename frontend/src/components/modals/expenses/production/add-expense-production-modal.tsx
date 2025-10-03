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
import { useState, useEffect, useMemo } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave, MdAttachFile } from "react-icons/md";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import FilePreview from "../../../common/file-preview";
import { useFarms } from "../../../../hooks/useFarms";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import type { AddExpenseProductionData } from "../../../../models/expenses/production/expenses-productions";
import { useExpensesContractor } from "../../../../hooks/expenses/useExpensesContractors";
import { ExpensesService } from "../../../../services/expenses-service";
import AppDialog from "../../../common/app-dialog";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";

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
  const stableFilters = useMemo(() => ({}), []);
  const {
    expensesContractors,
    loadingExpensesContractors,
    fetchExpensesContractors,
  } = useExpensesContractor(stableFilters);

  const [selectedFile, setSelectedFile] = useState<File>();
  const [loading, setLoading] = useState(false);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);

  const filePreviewUrl = selectedFile ? URL.createObjectURL(selectedFile) : "";

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<AddExpenseProductionData>({
    defaultValues: {
      farmId: "",
      cycleId: "",
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

  const watchedFarmId = watch("farmId");

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

  useEffect(() => {
    const fetchCyclesForFarm = async (farmId: string) => {
      setLoadingCycles(true);

      const selectedFarm = farms.find((f) => f.id === farmId);
      if (selectedFarm?.activeCycle) {
        setValue("cycleId", selectedFarm.activeCycle.id);
      } else {
        setValue("cycleId", "");
      }

      await handleApiResponse(
        () => FarmsService.getFarmCycles(farmId),
        (data) => {
          setCycles(data.responseData ?? []);
          setLoadingCycles(false);
        },
        () => {
          setCycles([]);
          setLoadingCycles(false);
        },
        "Nie udało się pobrać listy cykli."
      );
    };

    if (watchedFarmId && farms.length > 0) {
      fetchCyclesForFarm(watchedFarmId);
    } else {
      setCycles([]);
      setValue("cycleId", "");
    }
  }, [watchedFarmId, setValue, farms]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.[0]) {
      const file = e.target.files[0];
      setSelectedFile(file);
      setValue("file", file);
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
    setCycles([]);
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
                    loading={loadingCycles}
                    label="Cykl"
                    select
                    fullWidth
                    disabled={!watchedFarmId || cycles.length === 0}
                    value={watch("cycleId") || ""}
                    error={!!errors.cycleId}
                    helperText={errors.cycleId?.message}
                    {...register("cycleId", { required: "Cykl jest wymagany" })}
                  >
                    {cycles.map((cycle) => (
                      <MenuItem key={cycle.id} value={cycle.id}>
                        {`${cycle.identifier}/${cycle.year}`}
                      </MenuItem>
                    ))}
                  </LoadingTextField>
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
                    type="text"
                    inputMode="decimal"
                    slotProps={{ htmlInput: { step: "0.01" } }}
                    {...register("subTotal", {
                      required: "Wartość netto jest wymagana",
                      validate: (value) =>
                        !Number.isNaN(parseFloat(String(value))) ||
                        "Wartość musi być liczbą",
                      valueAsNumber: true,
                    })}
                    error={!!errors.subTotal}
                    helperText={errors.subTotal?.message}
                    fullWidth
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <TextField
                    label="VAT [zł]"
                    type="text"
                    inputMode="decimal"
                    slotProps={{ htmlInput: { step: "0.01" } }}
                    {...register("vatAmount", {
                      required: "VAT jest wymagany",
                      validate: (value) =>
                        !Number.isNaN(parseFloat(String(value))) ||
                        "Wartość musi być liczbą",
                      valueAsNumber: true,
                    })}
                    error={!!errors.vatAmount}
                    helperText={errors.vatAmount?.message}
                    fullWidth
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <TextField
                    label="Brutto [zł]"
                    type="text"
                    inputMode="decimal"
                    slotProps={{ htmlInput: { step: "0.01" } }}
                    {...register("invoiceTotal", {
                      required: "Wartość brutto jest wymagana",
                      validate: (value) =>
                        !Number.isNaN(parseFloat(String(value))) ||
                        "Wartość musi być liczbą",
                      valueAsNumber: true,
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
