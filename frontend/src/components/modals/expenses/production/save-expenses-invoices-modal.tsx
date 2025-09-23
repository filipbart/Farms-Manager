import {
  Typography,
  Button,
  DialogTitle,
  DialogContent,
  Grid,
  Divider,
  MenuItem,
  DialogActions,
  useTheme,
  useMediaQuery,
  TextField,
  Autocomplete,
} from "@mui/material";
import { useState, useEffect, useCallback } from "react";
import { MdSave, MdZoomIn } from "react-icons/md";
import { Controller, useForm } from "react-hook-form";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { toast } from "react-toastify";
import { useFarms } from "../../../../hooks/useFarms";
import { useExpensesContractor } from "../../../../hooks/expenses/useExpensesContractors";
import { useExpensesTypes } from "../../../../hooks/expenses/useExpensesTypes";
import { getFileTypeFromUrl } from "../../../../utils/fileUtils";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { ExpensesService } from "../../../../services/expenses-service";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import type {
  DraftExpenseInvoice,
  ExpenseInvoiceData,
} from "../../../../models/expenses/production/expenses-productions";
import AppDialog from "../../../common/app-dialog";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";

interface SaveExpensesInvoicesModalProps {
  open: boolean;
  onClose: () => void;
  draftExpenseInvoices: DraftExpenseInvoice[];
  onSave: (savedDraft: DraftExpenseInvoice) => void;
}

const SaveExpensesInvoicesModal: React.FC<SaveExpensesInvoicesModalProps> = ({
  open,
  onClose: handleClose,
  draftExpenseInvoices,
  onSave,
}) => {
  const theme = useTheme();
  const isMd = useMediaQuery(theme.breakpoints.up("md"));
  const isLg = useMediaQuery(theme.breakpoints.up("lg"));

  const [loading, setLoading] = useState(false);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [previewOpen, setPreviewOpen] = useState(false);

  const { farms, loadingFarms, fetchFarms } = useFarms();
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const {
    expensesContractors,
    loadingExpensesContractors,
    fetchExpensesContractors,
  } = useExpensesContractor({});
  const { expensesTypes, loadingExpensesTypes, fetchExpensesTypes } =
    useExpensesTypes();

  const [draftExpense, setDraftExpense] = useState<DraftExpenseInvoice>(
    draftExpenseInvoices[currentIndex]
  );

  const showExpenseTypeSelect = !draftExpense?.extractedFields?.expenseTypeName;

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<ExpenseInvoiceData>({
    defaultValues: {
      farmId: "",
      cycleId: "",
      contractorId: "",
      expenseTypeId: "",
      expenseTypeName: "",
      invoiceNumber: "",
      invoiceDate: "",
      subTotal: 0,
      vatAmount: 0,
      invoiceTotal: 0,
      comment: "",
    },
  });

  const watchedFarmId = watch("farmId");

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
    }
  }, [watchedFarmId, farms, setValue]);

  const handleContractorChange = useCallback(
    (contractorId: string) => {
      const selected = expensesContractors.find((c) => c.id === contractorId);
      setValue("expenseTypeName", selected?.expenseType || "");
      if (selected?.expenseType) {
        setValue("expenseTypeId", selected.expenseTypeId || "");
      }
    },
    [expensesContractors, setValue]
  );

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchExpensesContractors();
      if (
        draftExpenseInvoices.some((d) => !d.extractedFields.expenseTypeName)
      ) {
        fetchExpensesTypes();
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, draftExpenseInvoices]);

  useEffect(() => {
    const contractorName = draftExpense?.extractedFields.contractorName;
    const contractorIdFromBackend = draftExpense?.extractedFields.contractorId;

    if (contractorIdFromBackend) {
      handleContractorChange(contractorIdFromBackend);
      return;
    }

    if (contractorName && expensesContractors.length > 0) {
      const matchedContractor = expensesContractors.find(
        (c) => c.name.toLowerCase() === contractorName.toLowerCase()
      );
      if (matchedContractor) {
        handleContractorChange(matchedContractor.id);
      }
    }
  }, [expensesContractors, draftExpense, handleContractorChange]);

  useEffect(() => {
    if (draftExpenseInvoices.length === 0 && open) {
      handleClose();
      return;
    }
    const newIndex = Math.min(currentIndex, draftExpenseInvoices.length - 1);
    if (newIndex !== currentIndex) {
      setCurrentIndex(newIndex);
      return;
    }
    if (!farms.length) return;

    const currentDraft = draftExpenseInvoices[newIndex];
    setDraftExpense(currentDraft);
    const data = { ...currentDraft.extractedFields, comment: "" };
    reset(data);

    // useEffect for watchedFarmId will handle cycle logic
  }, [currentIndex, draftExpenseInvoices, farms, reset, open, handleClose]);

  const handleSave = async (formData: ExpenseInvoiceData) => {
    setLoading(true);
    await handleApiResponse(
      () =>
        ExpensesService.saveExpenseInvoice({
          draftId: draftExpense.draftId,
          filePath: draftExpense.filePath,
          data: formData,
        }),
      () => {
        toast.success(`Pomyślnie zapisano fakturę: ${formData.invoiceNumber}`);
        onSave(draftExpense);
      },
      (error) => {
        toast.error(
          error?.domainException?.errorDescription ||
            "Wystąpił błąd podczas zapisywania faktury"
        );
      }
    );
    setLoading(false);
  };

  const fileType = getFileTypeFromUrl(draftExpense?.fileUrl ?? "");

  const renderPreview = () => {
    if (!draftExpense?.fileUrl) return <Typography>Brak podglądu</Typography>;

    if (fileType === "pdf") {
      return (
        <>
          <iframe
            src={draftExpense.fileUrl}
            title="PDF Preview"
            width="100%"
            height={isLg ? "900px" : isMd ? "700px" : "500px"}
            style={{ border: "1px solid #ccc" }}
          />
          <Button
            startIcon={<MdZoomIn />}
            onClick={() => setPreviewOpen(true)}
            sx={{ mt: 1 }}
          >
            Powiększ
          </Button>
        </>
      );
    } else if (fileType === "image") {
      return (
        <>
          <img
            src={draftExpense.fileUrl}
            alt="Image Preview"
            style={{
              maxWidth: "100%",
              maxHeight: "900px",
              border: "1px solid #ccc",
            }}
          />
          <Button
            startIcon={<MdZoomIn />}
            onClick={() => setPreviewOpen(true)}
            sx={{ mt: 1 }}
          >
            Powiększ
          </Button>
        </>
      );
    } else {
      return <Typography>Nieobsługiwany format pliku</Typography>;
    }
  };

  if (!draftExpense) return null;

  return (
    <>
      <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="xl">
        <DialogTitle>Podgląd i weryfikacja faktury kosztowej</DialogTitle>
        <form onSubmit={handleSubmit(handleSave)}>
          <input type="hidden" {...register("cycleId")} />

          <DialogContent dividers>
            <Grid container spacing={3}>
              <Grid size={{ md: 12, lg: 5, xl: 6 }}>
                <Typography variant="h6">Podgląd faktury</Typography>
                {renderPreview()}
              </Grid>
              <Grid size={{ md: 12, lg: 7, xl: 6 }}>
                <Grid container spacing={2.5}>
                  <Grid size={12}>
                    <Typography variant="h6">Dane na fakturze</Typography>
                  </Grid>

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
                      {...register("cycleId", {
                        required: "Cykl jest wymagany",
                      })}
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
                      name="contractorId"
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
                    {showExpenseTypeSelect ? (
                      <Controller
                        name="expenseTypeId"
                        control={control}
                        rules={{ required: "Typ wydatku jest wymagany" }}
                        render={({ field }) => (
                          <LoadingTextField
                            {...field}
                            select
                            label="Typ wydatku"
                            fullWidth
                            loading={loadingExpensesTypes}
                            error={!!errors.expenseTypeId}
                            helperText={errors.expenseTypeId?.message}
                          >
                            {expensesTypes.map((type) => (
                              <MenuItem key={type.id} value={type.id}>
                                {type.name}
                              </MenuItem>
                            ))}
                          </LoadingTextField>
                        )}
                      />
                    ) : (
                      <TextField
                        label="Typ wydatku"
                        value={watch("expenseTypeName") || ""}
                        slotProps={{ input: { readOnly: true } }}
                        fullWidth
                      />
                    )}
                  </Grid>

                  <Grid size={12}>
                    <Divider sx={{ my: 1 }} />
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
                              date ? dayjs(date).format("YYYY-MM-DD") : null
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
                      type="number"
                      value={watch("subTotal") ?? ""}
                      slotProps={{ htmlInput: { step: "0.01", min: 0 } }}
                      {...register("subTotal", {
                        required: "Wartość netto jest wymagana",
                        valueAsNumber: true,
                        validate: (value) =>
                          value === undefined ||
                          value === undefined ||
                          value >= 0 ||
                          "Wartość netto nie może być ujemna",
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
                      value={watch("vatAmount") ?? ""}
                      slotProps={{ htmlInput: { step: "0.01", min: 0 } }}
                      {...register("vatAmount", {
                        required: "VAT jest wymagany",
                        valueAsNumber: true,
                        validate: (value) =>
                          value === undefined ||
                          value >= 0 ||
                          "Wartość VAT nie może być ujemna",
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
                      value={watch("invoiceTotal") ?? ""}
                      slotProps={{ htmlInput: { step: "0.01", min: 0 } }}
                      {...register("invoiceTotal", {
                        required: "Wartość brutto jest wymagana",
                        valueAsNumber: true,
                        validate: (value) =>
                          value === undefined ||
                          value >= 0 ||
                          "Wartość brutto nie może być ujemna",
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
            <Button
              onClick={() => setCurrentIndex((prev) => Math.max(0, prev - 1))}
              disabled={currentIndex === 0}
            >
              Poprzedni
            </Button>
            <Button
              onClick={() =>
                setCurrentIndex((prev) =>
                  Math.min(draftExpenseInvoices.length - 1, prev + 1)
                )
              }
              disabled={currentIndex === draftExpenseInvoices.length - 1}
            >
              Następny
            </Button>
            <Button onClick={handleClose} color="secondary" variant="outlined">
              Anuluj
            </Button>
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

      <AppDialog
        open={previewOpen}
        onClose={() => setPreviewOpen(false)}
        maxWidth="xl"
        fullWidth
      >
        <DialogContent sx={{ p: 0 }}>
          {fileType === "pdf" ? (
            <iframe
              src={`${draftExpense.fileUrl ?? ""}#zoom=FitH`}
              title="PDF Fullscreen"
              width="100%"
              height="1000vh"
              style={{ border: "none" }}
            />
          ) : fileType === "image" ? (
            <img
              src={draftExpense.fileUrl ?? ""}
              alt="Image Fullscreen"
              style={{
                width: "100%",
                height: "auto",
                maxHeight: "90vh",
                objectFit: "contain",
              }}
            />
          ) : (
            <Typography sx={{ p: 2 }}>Nieobsługiwany format pliku</Typography>
          )}
        </DialogContent>
      </AppDialog>
    </>
  );
};

export default SaveExpensesInvoicesModal;
