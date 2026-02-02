import {
  Autocomplete,
  Button,
  DialogActions,
  DialogContent,
  Grid,
  TextField,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  FormHelperText,
} from "@mui/material";
import DialogTitle from "@mui/material/DialogTitle";
import { useState, useEffect, useCallback, useMemo } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";

import LoadingButton from "../../../common/loading-button";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { ExpensesService } from "../../../../services/expenses-service";
import type { ExpenseTypeRow } from "../../../../models/expenses/expenses-types";
import type {
  ExpenseProductionListModel,
  UpdateExpenseProductionData,
} from "../../../../models/expenses/production/expenses-productions";
import AppDialog from "../../../common/app-dialog";
import { useFarms } from "../../../../hooks/useFarms";
import { useExpensesContractor } from "../../../../hooks/expenses/useExpensesContractors";
import LoadingTextField from "../../../common/loading-textfield";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";

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
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const stableFilters = useMemo(() => ({}), []);
  const {
    expensesContractors,
    loadingExpensesContractors,
    fetchExpensesContractors,
  } = useExpensesContractor(stableFilters);

  const [expenseTypes, setExpenseTypes] = useState<ExpenseTypeRow[]>([]);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [loading, setLoading] = useState(false);

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<UpdateExpenseProductionData>({
    defaultValues: {
      farmId: "",
      cycleId: "",
      expenseContractorId: "",
      expenseTypeId: "",
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
  const watchedContractorId = watch("expenseContractorId");

  useEffect(() => {
    if (open) {
      void fetchFarms();
      void fetchExpensesContractors();
      // Fetch all expense types as fallback
      handleApiResponse(
        () => ExpensesService.getExpensesTypes(),
        (data) => setExpenseTypes(data.responseData?.types || []),
        () => setExpenseTypes([]),
      );
    }
  }, [open, fetchFarms, fetchExpensesContractors]);

  const fetchCyclesForFarm = useCallback(async () => {
    const farmId = watch("farmId");
    if (!farmId) {
      setCycles([]);
      return;
    }
    setLoadingCycles(true);
    await handleApiResponse(
      () => FarmsService.getFarmCycles(farmId),
      (data) => {
        const fetchedCycles = data.responseData ?? [];
        setCycles(fetchedCycles);

        const currentCycleId = watch("cycleId");
        const isCurrentCycleValid = fetchedCycles.some(
          (c) => c.id === currentCycleId,
        );

        if (!isCurrentCycleValid) {
          const activeCycle = farms.find((f) => f.id === farmId)?.activeCycle;
          setValue("cycleId", activeCycle?.id || "", { shouldDirty: true });
        }
      },
      () => setCycles([]),
      "Nie udało się pobrać cykli dla wybranej fermy.",
    );
    setLoadingCycles(false);
  }, [watch, farms, setValue]);

  useEffect(() => {
    void fetchCyclesForFarm();
  }, [watchedFarmId, fetchCyclesForFarm]);

  useEffect(() => {
    if (expenseProductionToEdit) {
      reset(expenseProductionToEdit);
    }
  }, [expenseProductionToEdit, reset]);

  useEffect(() => {
    if (
      expenseProductionToEdit &&
      farms.length > 0 &&
      expensesContractors.length > 0
    ) {
      setValue("farmId", expenseProductionToEdit.farmId);
      setValue("cycleId", expenseProductionToEdit.cycleId);
      setValue(
        "expenseContractorId",
        expenseProductionToEdit.expenseContractorId,
      );
      setValue("expenseTypeId", expenseProductionToEdit.expenseTypeId || "");
      setValue(
        "expenseTypeNameDisplay",
        expenseProductionToEdit.expenseTypeName,
      );
    }
  }, [expenseProductionToEdit, farms, expensesContractors, setValue]);

  const handleContractorChange = (contractorId: string | null) => {
    const selected = expensesContractors.find((c) => c.id === contractorId);
    setValue(
      "expenseTypeNameDisplay",
      selected?.expenseTypes?.map((t) => t.name).join(", ") || "",
    );
    // Clear expense type if contractor changes and current type is not in new contractor's types
    if (selected) {
      const currentExpenseTypeId = watch("expenseTypeId");
      const hasCurrentType = selected.expenseTypes.some(
        (t) => t.id === currentExpenseTypeId,
      );
      if (!hasCurrentType) {
        setValue("expenseTypeId", "");
      }
    } else {
      setValue("expenseTypeId", "");
    }
  };

  const handleClose = () => {
    reset();
    setCycles([]);
    onClose();
  };

  const handleSave = async (data: UpdateExpenseProductionData) => {
    if (!expenseProductionToEdit) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        ExpensesService.updateExpenseProduction(
          expenseProductionToEdit.id,
          data,
        ),
      () => {
        toast.success("Pomyślnie zaktualizowano koszt produkcji");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji kosztu produkcji",
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
                  onChange: () => setValue("cycleId", ""),
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
              <Controller
                name="expenseContractorId"
                control={control}
                rules={{ required: "Kontrahent jest wymagany" }}
                render={({ field, fieldState: { error } }) => (
                  <Autocomplete
                    options={expensesContractors}
                    getOptionLabel={(option) => option.name || ""}
                    isOptionEqualToValue={(option, value) =>
                      option.id === value?.id
                    }
                    loading={loadingExpensesContractors}
                    value={
                      expensesContractors.find(
                        (contractor) => contractor.id === field.value,
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
              <Controller
                name="expenseTypeId"
                control={control}
                rules={{
                  required: "Typ wydatku jest wymagany",
                }}
                render={({ field }) => (
                  <FormControl
                    fullWidth
                    required
                    error={!!errors.expenseTypeId}
                  >
                    <InputLabel>Typ wydatku</InputLabel>
                    <Select
                      label="Typ wydatku"
                      value={field.value || ""}
                      onChange={field.onChange}
                      onBlur={field.onBlur}
                    >
                      {(() => {
                        const selectedContractor = expensesContractors.find(
                          (c) => c.id === watchedContractorId,
                        );

                        if (
                          selectedContractor &&
                          selectedContractor.expenseTypes.length > 0
                        ) {
                          return selectedContractor.expenseTypes.map((type) => (
                            <MenuItem key={type.id} value={type.id}>
                              {type.name}
                            </MenuItem>
                          ));
                        }

                        return expenseTypes.map((type) => (
                          <MenuItem key={type.id} value={type.id}>
                            {type.name}
                          </MenuItem>
                        ));
                      })()}
                    </Select>
                    {errors.expenseTypeId && (
                      <FormHelperText>
                        {errors.expenseTypeId.message}
                      </FormHelperText>
                    )}
                  </FormControl>
                )}
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
                        date ? dayjs(date).format("YYYY-MM-DD") : "",
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
                    !Number.isNaN(
                      parseFloat(String(value).replace(",", ".")),
                    ) || "Wartość musi być liczbą",
                  setValueAs: (v) =>
                    v === "" ? null : parseFloat(String(v).replace(",", ".")),
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
                    !Number.isNaN(
                      parseFloat(String(value).replace(",", ".")),
                    ) || "Wartość musi być liczbą",
                  setValueAs: (v) =>
                    v === "" ? null : parseFloat(String(v).replace(",", ".")),
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
                    !Number.isNaN(
                      parseFloat(String(value).replace(",", ".")),
                    ) || "Wartość musi być liczbą",
                  setValueAs: (v) =>
                    v === "" ? null : parseFloat(String(v).replace(",", ".")),
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
