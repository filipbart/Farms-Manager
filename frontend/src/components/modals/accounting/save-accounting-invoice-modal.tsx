import {
  Typography,
  Button,
  DialogTitle,
  DialogContent,
  Grid,
  Divider,
  DialogActions,
  useTheme,
  useMediaQuery,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormHelperText,
  Autocomplete,
} from "@mui/material";
import { useState, useEffect, useCallback } from "react";
import { MdSave } from "react-icons/md";
import type { DraftAccountingInvoice } from "../../../services/accounting-service";
import { AccountingService } from "../../../services/accounting-service";
import { FarmsService } from "../../../services/farms-service";
import { FeedsService } from "../../../services/feeds-service";
import { GasService } from "../../../services/gas-service";
import { ExpensesService } from "../../../services/expenses-service";
import { SlaughterhousesService } from "../../../services/slaughterhouses-service";
import FilePreview from "../../common/file-preview";
import { Controller, useForm } from "react-hook-form";
import LoadingButton from "../../common/loading-button";
import LoadingTextField from "../../common/loading-textfield";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { toast } from "react-toastify";
import AppDialog from "../../common/app-dialog";
import {
  ModuleType,
  ModuleTypeLabels,
  KSeFPaymentStatus,
  KSeFPaymentStatusLabels,
  KSeFInvoiceStatus,
  KSeFInvoiceStatusLabels,
  VatDeductionType,
  VatDeductionTypeLabels,
  InvoiceDocumentType,
  InvoiceDocumentTypeLabels,
} from "../../../models/accounting/ksef-invoice";
import type FarmRowModel from "../../../models/farms/farm-row-model";
import type CycleDto from "../../../models/farms/latest-cycle";
import type { GasContractorRow } from "../../../models/gas/gas-contractors";
import type { ExpenseContractorRow } from "../../../models/expenses/expenses-contractors";
import type { ExpenseTypeRow } from "../../../models/expenses/expenses-types";
import type { SlaughterhouseRowModel } from "../../../models/slaughterhouses/slaughterhouse-row-model";
import type { FeedsNamesRow } from "../../../models/feeds/feeds-names";

interface SaveAccountingInvoiceFormData {
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  sellerName: string;
  sellerNip: string;
  buyerName: string;
  buyerNip: string;
  grossAmount: number;
  netAmount: number;
  vatAmount: number;
  documentType: InvoiceDocumentType;
  status: KSeFInvoiceStatus;
  vatDeductionType: VatDeductionType;
  moduleType: ModuleType;
  paymentStatus: KSeFPaymentStatus;
  comment: string;
  // Feed module fields
  feedFarmId: string;
  feedCycleId: string;
  feedHenhouseId: string;
  feedBankAccountNumber: string;
  feedVendorName: string;
  feedItemName: string;
  feedQuantity: number;
  feedUnitPrice: number;
  // Gas module fields
  gasFarmId: string;
  gasContractorId: string;
  gasUnitPrice: number;
  gasQuantity: number;
  // Expense module fields
  expenseFarmId: string;
  expenseCycleId: string;
  expenseContractorId: string;
  expenseTypeId: string;
  // Sale module fields
  saleFarmId: string;
  saleCycleId: string;
  saleSlaughterhouseId: string;
}

interface SaveAccountingInvoiceModalProps {
  open: boolean;
  onClose: () => void;
  draftInvoices: DraftAccountingInvoice[];
  onSave: (savedInvoice: DraftAccountingInvoice) => void;
}

const SaveAccountingInvoiceModal: React.FC<SaveAccountingInvoiceModalProps> = ({
  open,
  onClose: handleClose,
  draftInvoices,
  onSave,
}) => {
  const theme = useTheme();
  const isMd = useMediaQuery(theme.breakpoints.up("md"));
  const isLg = useMediaQuery(theme.breakpoints.up("lg"));

  const [loading, setLoading] = useState(false);
  const [currentIndex, setCurrentIndex] = useState(0);

  // Module data states
  const [farms, setFarms] = useState<FarmRowModel[]>([]);
  const [loadingFarms, setLoadingFarms] = useState(false);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [henhouses, setHenhouses] = useState<{ id: string; name: string }[]>(
    []
  );
  const [feedNames, setFeedNames] = useState<FeedsNamesRow[]>([]);
  const [gasContractors, setGasContractors] = useState<GasContractorRow[]>([]);
  const [expenseContractors, setExpenseContractors] = useState<
    ExpenseContractorRow[]
  >([]);
  const [expenseTypes, setExpenseTypes] = useState<ExpenseTypeRow[]>([]);
  const [slaughterhouses, setSlaughterhouses] = useState<
    SlaughterhouseRowModel[]
  >([]);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    watch,
    setValue,
  } = useForm<SaveAccountingInvoiceFormData>();

  const watchedModuleType = watch("moduleType");
  const watchedFeedFarmId = watch("feedFarmId");
  const watchedGasFarmId = watch("gasFarmId");
  const watchedExpenseFarmId = watch("expenseFarmId");
  const watchedSaleFarmId = watch("saleFarmId");
  const watchedExpenseContractorId = watch("expenseContractorId");

  const draftInvoice = draftInvoices[currentIndex];

  // Fetch farms on open
  const fetchFarms = useCallback(async () => {
    setLoadingFarms(true);
    await handleApiResponse(
      () => FarmsService.getFarmsAsync(),
      (data) => setFarms(data.responseData?.items || []),
      () => setFarms([])
    );
    setLoadingFarms(false);
  }, []);

  // Fetch cycles when farm changes
  const fetchCycles = useCallback(async (farmId: string) => {
    if (!farmId) {
      setCycles([]);
      return;
    }
    setLoadingCycles(true);
    await handleApiResponse(
      () => FarmsService.getFarmCycles(farmId),
      (data) => setCycles(data.responseData ?? []),
      () => setCycles([])
    );
    setLoadingCycles(false);
  }, []);

  // Update henhouses when feed farm changes
  useEffect(() => {
    if (watchedFeedFarmId) {
      const farm = farms.find((f) => f.id === watchedFeedFarmId);
      setHenhouses(farm?.henhouses || []);
      fetchCycles(watchedFeedFarmId);
    }
  }, [watchedFeedFarmId, farms, fetchCycles]);

  // Update cycles when other module farm changes
  useEffect(() => {
    const farmId =
      watchedGasFarmId || watchedExpenseFarmId || watchedSaleFarmId;
    if (farmId && !watchedFeedFarmId) {
      fetchCycles(farmId);
    }
  }, [
    watchedGasFarmId,
    watchedExpenseFarmId,
    watchedSaleFarmId,
    watchedFeedFarmId,
    fetchCycles,
  ]);

  // Fetch module-specific data
  useEffect(() => {
    if (!open) return;

    fetchFarms();

    // Feed names
    handleApiResponse(
      () => FeedsService.getFeedsNames(),
      (data) => setFeedNames(data.responseData?.fields ?? []),
      () => setFeedNames([])
    );

    // Gas contractors
    handleApiResponse(
      () => GasService.getGasContractors(),
      (data) => setGasContractors(data.responseData?.contractors ?? []),
      () => setGasContractors([])
    );

    // Expense contractors
    handleApiResponse(
      () => ExpensesService.getExpensesContractors({}),
      (data) => setExpenseContractors(data.responseData?.contractors ?? []),
      () => setExpenseContractors([])
    );

    // Expense types
    handleApiResponse(
      () => ExpensesService.getExpensesTypes(),
      (data) => setExpenseTypes(data.responseData?.types ?? []),
      () => setExpenseTypes([])
    );

    // Slaughterhouses
    handleApiResponse(
      () => SlaughterhousesService.getAllSlaughterhouses(),
      (data) => setSlaughterhouses(data.responseData?.items ?? []),
      () => setSlaughterhouses([])
    );
  }, [open, fetchFarms]);

  useEffect(() => {
    if (draftInvoices.length === 0 && open) {
      handleClose();
      return;
    }

    const newIndex = Math.min(currentIndex, draftInvoices.length - 1);
    if (newIndex !== currentIndex) {
      setCurrentIndex(newIndex);
      return;
    }

    const currentDraft = draftInvoices[newIndex];
    if (currentDraft) {
      reset({
        invoiceNumber: currentDraft.extractedFields.invoiceNumber || "",
        invoiceDate: currentDraft.extractedFields.invoiceDate || "",
        dueDate: currentDraft.extractedFields.dueDate || "",
        sellerName: currentDraft.extractedFields.sellerName || "",
        sellerNip: currentDraft.extractedFields.sellerNip || "",
        buyerName: currentDraft.extractedFields.buyerName || "",
        buyerNip: currentDraft.extractedFields.buyerNip || "",
        grossAmount: currentDraft.extractedFields.grossAmount || 0,
        netAmount: currentDraft.extractedFields.netAmount || 0,
        vatAmount: currentDraft.extractedFields.vatAmount || 0,
        documentType: InvoiceDocumentType.Vat,
        status: KSeFInvoiceStatus.Accepted,
        vatDeductionType: VatDeductionType.Full,
        moduleType: ModuleType.None,
        paymentStatus: KSeFPaymentStatus.Unpaid,
        comment: "",
      });
    }
  }, [currentIndex, draftInvoices, reset, open, handleClose]);

  const handleSave = async (formData: SaveAccountingInvoiceFormData) => {
    if (!draftInvoice) return;

    // Build module-specific data
    let feedData: Parameters<
      typeof AccountingService.saveInvoice
    >[0]["feedData"];
    let gasData: Parameters<typeof AccountingService.saveInvoice>[0]["gasData"];
    let expenseData: Parameters<
      typeof AccountingService.saveInvoice
    >[0]["expenseData"];
    let saleData: Parameters<
      typeof AccountingService.saveInvoice
    >[0]["saleData"];

    switch (formData.moduleType) {
      case ModuleType.Feeds:
        feedData = {
          farmId: formData.feedFarmId,
          cycleId: formData.feedCycleId,
          henhouseId: formData.feedHenhouseId,
          bankAccountNumber: formData.feedBankAccountNumber,
          vendorName: formData.feedVendorName || formData.sellerName,
          itemName: formData.feedItemName,
          quantity: formData.feedQuantity,
          unitPrice: formData.feedUnitPrice,
        };
        break;
      case ModuleType.Gas:
        gasData = {
          farmId: formData.gasFarmId,
          contractorId: formData.gasContractorId || undefined,
          unitPrice: formData.gasUnitPrice,
          quantity: formData.gasQuantity,
        };
        break;
      case ModuleType.ProductionExpenses:
        expenseData = {
          farmId: formData.expenseFarmId,
          cycleId: formData.expenseCycleId,
          expenseContractorId: formData.expenseContractorId || undefined,
          expenseTypeId: formData.expenseTypeId,
        };
        break;
      case ModuleType.Sales:
        saleData = {
          farmId: formData.saleFarmId,
          cycleId: formData.saleCycleId,
          slaughterhouseId: formData.saleSlaughterhouseId || undefined,
        };
        break;
    }

    setLoading(true);
    await handleApiResponse(
      () =>
        AccountingService.saveInvoice({
          draftId: draftInvoice.draftId,
          filePath: draftInvoice.filePath,
          invoiceNumber: formData.invoiceNumber,
          invoiceDate: formData.invoiceDate,
          dueDate: formData.dueDate || undefined,
          sellerName: formData.sellerName,
          sellerNip: formData.sellerNip,
          buyerName: formData.buyerName,
          buyerNip: formData.buyerNip,
          grossAmount: formData.grossAmount,
          netAmount: formData.netAmount,
          vatAmount: formData.vatAmount,
          invoiceType: draftInvoice.extractedFields.invoiceType,
          documentType: formData.documentType,
          status: formData.status,
          vatDeductionType: formData.vatDeductionType,
          moduleType: formData.moduleType,
          paymentStatus: formData.paymentStatus,
          comment: formData.comment || undefined,
          feedData,
          gasData,
          expenseData,
          saleData,
        }),
      () => {
        toast.success(`Pomyślnie zapisano fakturę: ${formData.invoiceNumber}`);
        onSave(draftInvoice);
      },
      undefined,
      "Wystąpił błąd podczas zapisywania faktury"
    );
    setLoading(false);
  };

  const renderPreview = () => {
    if (!draftInvoice?.fileUrl) return <Typography>Brak podglądu</Typography>;

    return (
      <FilePreview
        file={draftInvoice.fileUrl}
        maxHeight={isLg ? 900 : isMd ? 700 : 500}
        showPreviewButton={true}
      />
    );
  };

  if (!draftInvoice) {
    return null;
  }

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="xl">
      <DialogTitle>
        Podgląd faktury i dane ({currentIndex + 1} / {draftInvoices.length})
      </DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent dividers>
          <Grid container spacing={2}>
            <Grid size={{ md: 12, lg: 5, xl: 6 }}>
              <Typography variant="h6">Podgląd faktury</Typography>
              {renderPreview()}
            </Grid>

            <Grid size={{ md: 12, lg: 7, xl: 6 }}>
              <Grid container spacing={3} alignItems={"top"}>
                <Grid size={12}>
                  <Typography variant="h6">Dane zaczytane z faktury</Typography>
                  <Typography variant="body2" color="text.secondary">
                    Sprawdź i popraw dane, jeśli AI zaczytało je niepoprawnie
                  </Typography>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Numer faktury"
                    error={!!errors.invoiceNumber}
                    helperText={errors.invoiceNumber?.message}
                    {...register("invoiceNumber", {
                      required: "Numer faktury jest wymagany",
                    })}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
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
                        label="Data wystawienia"
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
                          },
                        }}
                      />
                    )}
                  />
                </Grid>

                <Grid size={12}>
                  <Divider>
                    <Typography variant="caption">Sprzedawca</Typography>
                  </Divider>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Nazwa sprzedawcy"
                    error={!!errors.sellerName}
                    helperText={errors.sellerName?.message}
                    {...register("sellerName", {
                      required: "Nazwa sprzedawcy jest wymagana",
                    })}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="NIP sprzedawcy"
                    error={!!errors.sellerNip}
                    helperText={errors.sellerNip?.message}
                    {...register("sellerNip")}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Grid>

                <Grid size={12}>
                  <Divider>
                    <Typography variant="caption">Nabywca</Typography>
                  </Divider>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Nazwa nabywcy"
                    error={!!errors.buyerName}
                    helperText={errors.buyerName?.message}
                    {...register("buyerName", {
                      required: "Nazwa nabywcy jest wymagana",
                    })}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="NIP nabywcy"
                    error={!!errors.buyerNip}
                    helperText={errors.buyerNip?.message}
                    {...register("buyerNip")}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Grid>

                <Grid size={12}>
                  <Divider>
                    <Typography variant="caption">Kwoty</Typography>
                  </Divider>
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <TextField
                    label="Kwota netto [zł]"
                    type="number"
                    slotProps={{
                      htmlInput: { min: 0, step: "0.01" },
                      inputLabel: { shrink: true },
                    }}
                    error={!!errors.netAmount}
                    helperText={errors.netAmount?.message}
                    {...register("netAmount", {
                      required: "Kwota netto jest wymagana",
                      valueAsNumber: true,
                    })}
                    fullWidth
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <TextField
                    label="Kwota VAT [zł]"
                    type="number"
                    slotProps={{
                      htmlInput: { min: 0, step: "0.01" },
                      inputLabel: { shrink: true },
                    }}
                    error={!!errors.vatAmount}
                    helperText={errors.vatAmount?.message}
                    {...register("vatAmount", {
                      required: "Kwota VAT jest wymagana",
                      valueAsNumber: true,
                    })}
                    fullWidth
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <TextField
                    label="Kwota brutto [zł]"
                    type="number"
                    slotProps={{
                      htmlInput: { min: 0, step: "0.01" },
                      inputLabel: { shrink: true },
                    }}
                    error={!!errors.grossAmount}
                    helperText={errors.grossAmount?.message}
                    {...register("grossAmount", {
                      required: "Kwota brutto jest wymagana",
                      valueAsNumber: true,
                    })}
                    fullWidth
                  />
                </Grid>

                <Grid size={12}>
                  <Divider>
                    <Typography variant="caption">Przypisanie</Typography>
                  </Divider>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <FormControl fullWidth error={!!errors.moduleType} required>
                    <InputLabel id="module-type-label">Moduł</InputLabel>
                    <Select
                      labelId="module-type-label"
                      label="Moduł"
                      value={watchedModuleType || ""}
                      {...register("moduleType", {
                        required: "Moduł jest wymagany",
                        validate: (value) =>
                          value !== ModuleType.None || "Wybierz moduł",
                      })}
                    >
                      {Object.entries(ModuleTypeLabels)
                        .filter(([key]) => key !== ModuleType.None)
                        .map(([key, label]) => (
                          <MenuItem key={key} value={key}>
                            {label}
                          </MenuItem>
                        ))}
                    </Select>
                    {errors.moduleType && (
                      <FormHelperText>
                        {errors.moduleType.message}
                      </FormHelperText>
                    )}
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <FormControl fullWidth>
                    <InputLabel id="payment-status-label">
                      Status płatności
                    </InputLabel>
                    <Select
                      labelId="payment-status-label"
                      label="Status płatności"
                      defaultValue={KSeFPaymentStatus.Unpaid}
                      {...register("paymentStatus")}
                    >
                      {Object.entries(KSeFPaymentStatusLabels).map(
                        ([key, label]) => (
                          <MenuItem key={key} value={key}>
                            {label}
                          </MenuItem>
                        )
                      )}
                    </Select>
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <FormControl fullWidth>
                    <InputLabel id="document-type-label">
                      Typ dokumentu
                    </InputLabel>
                    <Select
                      labelId="document-type-label"
                      label="Typ dokumentu"
                      defaultValue={InvoiceDocumentType.Vat}
                      {...register("documentType")}
                    >
                      {Object.entries(InvoiceDocumentTypeLabels).map(
                        ([key, label]) => (
                          <MenuItem key={key} value={key}>
                            {label}
                          </MenuItem>
                        )
                      )}
                    </Select>
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <FormControl fullWidth>
                    <InputLabel id="invoice-status-label">
                      Status faktury
                    </InputLabel>
                    <Select
                      labelId="invoice-status-label"
                      label="Status faktury"
                      defaultValue={KSeFInvoiceStatus.Accepted}
                      {...register("status")}
                    >
                      {Object.entries(KSeFInvoiceStatusLabels).map(
                        ([key, label]) => (
                          <MenuItem key={key} value={key}>
                            {label}
                          </MenuItem>
                        )
                      )}
                    </Select>
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <FormControl fullWidth>
                    <InputLabel id="vat-deduction-label">
                      Odliczenie VAT
                    </InputLabel>
                    <Select
                      labelId="vat-deduction-label"
                      label="Odliczenie VAT"
                      defaultValue={VatDeductionType.Full}
                      {...register("vatDeductionType")}
                    >
                      {Object.entries(VatDeductionTypeLabels).map(
                        ([key, label]) => (
                          <MenuItem key={key} value={key}>
                            {label}
                          </MenuItem>
                        )
                      )}
                    </Select>
                  </FormControl>
                </Grid>

                {/* Module-specific fields */}
                {watchedModuleType === ModuleType.Feeds && (
                  <>
                    <Grid size={12}>
                      <Divider>
                        <Typography variant="caption">
                          Dane dostawy paszy
                        </Typography>
                      </Divider>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <LoadingTextField
                        loading={loadingFarms}
                        select
                        label="Ferma"
                        fullWidth
                        required
                        value={watchedFeedFarmId || ""}
                        error={!!errors.feedFarmId}
                        helperText={errors.feedFarmId?.message}
                        {...register("feedFarmId", {
                          required: "Ferma jest wymagana",
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
                        select
                        label="Cykl"
                        fullWidth
                        required
                        disabled={!watchedFeedFarmId}
                        value={watch("feedCycleId") || ""}
                        error={!!errors.feedCycleId}
                        helperText={errors.feedCycleId?.message}
                        {...register("feedCycleId", {
                          required: "Cykl jest wymagany",
                        })}
                      >
                        {cycles.map((cycle) => (
                          <MenuItem key={cycle.id} value={cycle.id}>
                            {cycle.identifier}/{cycle.year}
                          </MenuItem>
                        ))}
                      </LoadingTextField>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        select
                        label="Kurnik"
                        fullWidth
                        required
                        disabled={!watchedFeedFarmId}
                        value={watch("feedHenhouseId") || ""}
                        error={!!errors.feedHenhouseId}
                        helperText={errors.feedHenhouseId?.message}
                        {...register("feedHenhouseId", {
                          required: "Kurnik jest wymagany",
                        })}
                      >
                        {henhouses.map((h) => (
                          <MenuItem key={h.id} value={h.id}>
                            {h.name}
                          </MenuItem>
                        ))}
                      </TextField>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Autocomplete
                        freeSolo
                        options={feedNames.map((f) => f.name)}
                        value={watch("feedItemName") || ""}
                        onChange={(_, value) =>
                          setValue("feedItemName", value || "")
                        }
                        onInputChange={(_, value) =>
                          setValue("feedItemName", value)
                        }
                        renderInput={(params) => (
                          <TextField
                            {...params}
                            label="Nazwa paszy"
                            required
                            error={!!errors.feedItemName}
                            helperText={errors.feedItemName?.message}
                          />
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        label="Ilość [kg]"
                        type="number"
                        fullWidth
                        required
                        slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                        error={!!errors.feedQuantity}
                        helperText={errors.feedQuantity?.message}
                        {...register("feedQuantity", {
                          required: "Ilość jest wymagana",
                          valueAsNumber: true,
                        })}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        label="Cena jednostkowa [zł/kg]"
                        type="number"
                        fullWidth
                        required
                        slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                        error={!!errors.feedUnitPrice}
                        helperText={errors.feedUnitPrice?.message}
                        {...register("feedUnitPrice", {
                          required: "Cena jest wymagana",
                          valueAsNumber: true,
                        })}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        label="Dostawca"
                        fullWidth
                        slotProps={{ inputLabel: { shrink: true } }}
                        {...register("feedVendorName")}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        label="Nr konta bankowego"
                        fullWidth
                        {...register("feedBankAccountNumber")}
                      />
                    </Grid>
                  </>
                )}

                {watchedModuleType === ModuleType.Gas && (
                  <>
                    <Grid size={12}>
                      <Divider>
                        <Typography variant="caption">
                          Dane dostawy gazu
                        </Typography>
                      </Divider>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <LoadingTextField
                        loading={loadingFarms}
                        select
                        label="Ferma"
                        fullWidth
                        required
                        value={watchedGasFarmId || ""}
                        error={!!errors.gasFarmId}
                        helperText={errors.gasFarmId?.message}
                        {...register("gasFarmId", {
                          required: "Ferma jest wymagana",
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
                      <Autocomplete
                        options={gasContractors}
                        getOptionLabel={(option) => option.name || ""}
                        value={
                          gasContractors.find(
                            (c) => c.id === watch("gasContractorId")
                          ) || null
                        }
                        onChange={(_, value) =>
                          setValue("gasContractorId", value?.id || "")
                        }
                        renderInput={(params) => (
                          <TextField {...params} label="Dostawca gazu" />
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        label="Ilość [m³]"
                        type="number"
                        fullWidth
                        required
                        slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                        error={!!errors.gasQuantity}
                        helperText={errors.gasQuantity?.message}
                        {...register("gasQuantity", {
                          required: "Ilość jest wymagana",
                          valueAsNumber: true,
                        })}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        label="Cena jednostkowa [zł/m³]"
                        type="number"
                        fullWidth
                        required
                        slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                        error={!!errors.gasUnitPrice}
                        helperText={errors.gasUnitPrice?.message}
                        {...register("gasUnitPrice", {
                          required: "Cena jest wymagana",
                          valueAsNumber: true,
                        })}
                      />
                    </Grid>
                  </>
                )}

                {watchedModuleType === ModuleType.ProductionExpenses && (
                  <>
                    <Grid size={12}>
                      <Divider>
                        <Typography variant="caption">
                          Dane kosztu produkcyjnego
                        </Typography>
                      </Divider>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <LoadingTextField
                        loading={loadingFarms}
                        select
                        label="Ferma"
                        fullWidth
                        required
                        value={watchedExpenseFarmId || ""}
                        error={!!errors.expenseFarmId}
                        helperText={errors.expenseFarmId?.message}
                        {...register("expenseFarmId", {
                          required: "Ferma jest wymagana",
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
                        select
                        label="Cykl"
                        fullWidth
                        required
                        disabled={!watchedExpenseFarmId}
                        value={watch("expenseCycleId") || ""}
                        error={!!errors.expenseCycleId}
                        helperText={errors.expenseCycleId?.message}
                        {...register("expenseCycleId", {
                          required: "Cykl jest wymagany",
                        })}
                      >
                        {cycles.map((cycle) => (
                          <MenuItem key={cycle.id} value={cycle.id}>
                            {cycle.identifier}/{cycle.year}
                          </MenuItem>
                        ))}
                      </LoadingTextField>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Autocomplete
                        options={expenseContractors}
                        getOptionLabel={(option) => option.name || ""}
                        value={
                          expenseContractors.find(
                            (c) => c.id === watchedExpenseContractorId
                          ) || null
                        }
                        onChange={(_, value) => {
                          setValue("expenseContractorId", value?.id || "");
                          setValue("expenseTypeId", "");
                        }}
                        renderInput={(params) => (
                          <TextField {...params} label="Kontrahent" />
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <FormControl
                        fullWidth
                        required
                        error={!!errors.expenseTypeId}
                      >
                        <InputLabel>Typ wydatku</InputLabel>
                        <Select
                          label="Typ wydatku"
                          value={watch("expenseTypeId") || ""}
                          {...register("expenseTypeId", {
                            required: "Typ wydatku jest wymagany",
                          })}
                        >
                          {(() => {
                            const selectedContractor = expenseContractors.find(
                              (c) => c.id === watchedExpenseContractorId
                            );
                            const availableTypes =
                              selectedContractor?.expenseTypes || [];
                            if (availableTypes.length > 0) {
                              return availableTypes.map((type) => (
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
                    </Grid>
                  </>
                )}

                {watchedModuleType === ModuleType.Sales && (
                  <>
                    <Grid size={12}>
                      <Divider>
                        <Typography variant="caption">
                          Dane faktury sprzedaży
                        </Typography>
                      </Divider>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <LoadingTextField
                        loading={loadingFarms}
                        select
                        label="Ferma"
                        fullWidth
                        required
                        value={watchedSaleFarmId || ""}
                        error={!!errors.saleFarmId}
                        helperText={errors.saleFarmId?.message}
                        {...register("saleFarmId", {
                          required: "Ferma jest wymagana",
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
                        select
                        label="Cykl"
                        fullWidth
                        required
                        disabled={!watchedSaleFarmId}
                        value={watch("saleCycleId") || ""}
                        error={!!errors.saleCycleId}
                        helperText={errors.saleCycleId?.message}
                        {...register("saleCycleId", {
                          required: "Cykl jest wymagany",
                        })}
                      >
                        {cycles.map((cycle) => (
                          <MenuItem key={cycle.id} value={cycle.id}>
                            {cycle.identifier}/{cycle.year}
                          </MenuItem>
                        ))}
                      </LoadingTextField>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Autocomplete
                        options={slaughterhouses}
                        getOptionLabel={(option) => option.name || ""}
                        value={
                          slaughterhouses.find(
                            (s) => s.id === watch("saleSlaughterhouseId")
                          ) || null
                        }
                        onChange={(_, value) =>
                          setValue("saleSlaughterhouseId", value?.id || "")
                        }
                        renderInput={(params) => (
                          <TextField {...params} label="Ubojnia" required />
                        )}
                      />
                    </Grid>
                  </>
                )}

                <Grid size={12}>
                  <TextField
                    label="Komentarz"
                    error={!!errors.comment}
                    helperText={errors.comment?.message}
                    {...register("comment")}
                    fullWidth
                    multiline
                    rows={2}
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
                Math.min(draftInvoices.length - 1, prev + 1)
              )
            }
            disabled={currentIndex === draftInvoices.length - 1}
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

export default SaveAccountingInvoiceModal;
