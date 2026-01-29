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
import { useState, useEffect, useCallback, useRef } from "react";
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
import {
  formatNumberWithSpaces,
  parseFormattedNumber,
} from "../../../utils/number-format";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { toast } from "react-toastify";
import AppDialog from "../../common/app-dialog";
import { FilesService } from "../../../services/files-service";
import { FileType } from "../../../models/files/file-type";
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
import { UsersService } from "../../../services/users-service";
import type { UserListModel } from "../../../models/users/users";
import { useAuth } from "../../../auth/useAuth";

interface SaveAccountingInvoiceFormData {
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  sellerName: string;
  sellerNip: string;
  buyerName: string;
  buyerNip: string;
  grossAmount: string | number;
  netAmount: string | number;
  vatAmount: string | number;
  documentType: InvoiceDocumentType;
  status: KSeFInvoiceStatus;
  vatDeductionType: VatDeductionType;
  moduleType: ModuleType;
  paymentStatus: KSeFPaymentStatus;
  paymentDate: string;
  comment: string;
  assignedUserId: string;
  relatedInvoiceNumber: string;
  // Feed module fields
  feedFarmId: string;
  feedCycleId: string;
  feedHenhouseId: string;
  feedBankAccountNumber: string;
  feedVendorName: string;
  feedItemName: string;
  feedQuantity: string | number;
  feedUnitPrice: string | number;
  // Gas module fields
  gasFarmId: string;
  gasCycleId: string;
  gasContractorId: string;
  gasUnitPrice: string | number;
  gasQuantity: string | number;
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
  const { userData } = useAuth();

  const [loading, setLoading] = useState(false);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [previewFile, setPreviewFile] = useState<File | null>(null);
  const [savedCount, setSavedCount] = useState(0);
  const isResettingRef = useRef(false);

  // Module data states
  const [farms, setFarms] = useState<FarmRowModel[]>([]);
  const [loadingFarms, setLoadingFarms] = useState(false);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [henhouses, setHenhouses] = useState<{ id: string; name: string }[]>(
    [],
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
  const [users, setUsers] = useState<UserListModel[]>([]);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    watch,
    setValue,
  } = useForm<SaveAccountingInvoiceFormData>({
    shouldUnregister: false,
    mode: "onSubmit",
  });

  const watchedModuleType = watch("moduleType");
  const watchedFeedFarmId = watch("feedFarmId");
  const watchedFeedQuantity = watch("feedQuantity");
  const watchedFeedUnitPrice = watch("feedUnitPrice");
  const watchedGasFarmId = watch("gasFarmId");
  const watchedExpenseFarmId = watch("expenseFarmId");
  const watchedSaleFarmId = watch("saleFarmId");
  const watchedExpenseContractorId = watch("expenseContractorId");
  const watchedPaymentStatus = watch("paymentStatus");
  const watchedInvoiceDate = watch("invoiceDate");
  const watchedNetAmount = watch("netAmount");
  const watchedVatAmount = watch("vatAmount");
  const watchedGrossAmount = watch("grossAmount");

  const draftInvoice = draftInvoices[currentIndex];

  // Fetch file blob for preview
  useEffect(() => {
    let active = true;
    const fetchPreview = async () => {
      setPreviewFile(null);
      if (!draftInvoice?.filePath) {
        return;
      }

      try {
        const blob = await FilesService.getFile(
          draftInvoice.filePath,
          FileType.AccountingInvoice,
        );

        if (active && blob) {
          const fileName = draftInvoice.filePath.split("/").pop() || "invoice";
          setPreviewFile(new File([blob], fileName, { type: blob.type }));
        }
      } catch (err) {
        console.error("Failed to fetch preview blob", err);
      }
    };

    fetchPreview();

    return () => {
      active = false;
    };
  }, [draftInvoice?.filePath]);

  // Fetch farms on open
  const fetchFarms = useCallback(async () => {
    setLoadingFarms(true);
    await handleApiResponse(
      () => FarmsService.getFarmsAsync(),
      (data) => setFarms(data.responseData?.items || []),
      () => setFarms([]),
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
      () => setCycles([]),
    );
    setLoadingCycles(false);
  }, []);

  // Update henhouses and cycles when feed farm changes
  useEffect(() => {
    if (watchedFeedFarmId && farms.length > 0) {
      const farm = farms.find((f) => f.id === watchedFeedFarmId);
      setHenhouses(farm?.henhouses || []);
      fetchCycles(watchedFeedFarmId);
      // Auto-select active cycle
      if (farm?.activeCycle) {
        setValue("feedCycleId", farm.activeCycle.id);
      }
    }
  }, [watchedFeedFarmId, farms, fetchCycles, setValue]);

  // Update cycles when gas farm changes and auto-select active cycle
  useEffect(() => {
    if (watchedGasFarmId && farms.length > 0) {
      const farm = farms.find((f) => f.id === watchedGasFarmId);
      fetchCycles(watchedGasFarmId);
      // Auto-select active cycle for production expense entry
      if (farm?.activeCycle) {
        setValue("gasCycleId", farm.activeCycle.id);
      }
    }
  }, [watchedGasFarmId, farms, fetchCycles, setValue]);

  // Update cycles when expense farm changes and auto-select active cycle
  useEffect(() => {
    if (watchedExpenseFarmId && farms.length > 0) {
      const farm = farms.find((f) => f.id === watchedExpenseFarmId);
      fetchCycles(watchedExpenseFarmId);
      if (farm?.activeCycle) {
        setValue("expenseCycleId", farm.activeCycle.id);
      }
    }
  }, [watchedExpenseFarmId, farms, fetchCycles, setValue]);

  // Update cycles when sale farm changes and auto-select active cycle
  useEffect(() => {
    if (watchedSaleFarmId && farms.length > 0) {
      const farm = farms.find((f) => f.id === watchedSaleFarmId);
      fetchCycles(watchedSaleFarmId);
      if (farm?.activeCycle) {
        setValue("saleCycleId", farm.activeCycle.id);
      }
    }
  }, [watchedSaleFarmId, farms, fetchCycles, setValue]);

  // Auto-fill module-specific fields when moduleType changes
  useEffect(() => {
    if (!draftInvoice || !farms.length) return;
    const ef = draftInvoice.extractedFields;

    switch (watchedModuleType) {
      case ModuleType.Feeds:
        if (ef.farmId) setValue("feedFarmId", ef.farmId);
        if (ef.cycleId) setValue("feedCycleId", ef.cycleId);
        if (ef.henhouseId) setValue("feedHenhouseId", ef.henhouseId);
        // Match feed name from seller
        if (feedNames.length > 0) {
          const matchedFeedName = feedNames.find(
            (f) => f.name.toLowerCase() === ef.sellerName?.toLowerCase(),
          );
          if (matchedFeedName) setValue("feedItemName", matchedFeedName.name);
        }
        setValue("feedVendorName", ef.sellerName || "");
        break;
      case ModuleType.Gas:
        if (ef.farmId) setValue("gasFarmId", ef.farmId);
        break;
      case ModuleType.ProductionExpenses:
        if (ef.farmId) setValue("expenseFarmId", ef.farmId);
        if (ef.cycleId) setValue("expenseCycleId", ef.cycleId);
        break;
      case ModuleType.Sales:
        if (ef.farmId) setValue("saleFarmId", ef.farmId);
        if (ef.cycleId) setValue("saleCycleId", ef.cycleId);
        break;
    }
  }, [watchedModuleType, draftInvoice, farms, feedNames, setValue]);

  // Auto-fill gas contractor (like in save-gas-invoices-modal.tsx)
  useEffect(() => {
    if (watchedModuleType !== ModuleType.Gas || !draftInvoice) return;
    const ef = draftInvoice.extractedFields;

    // First check if backend returned contractor ID (new or existing)
    if (ef.gasContractorId) {
      setValue("gasContractorId", ef.gasContractorId);
      return;
    }

    // Try to match by seller name
    if (gasContractors.length > 0 && ef.sellerName) {
      const matchedContractor = gasContractors.find(
        (c) => c.name.toLowerCase() === ef.sellerName?.toLowerCase(),
      );
      if (matchedContractor) {
        setValue("gasContractorId", matchedContractor.id);
      }
    }
  }, [watchedModuleType, draftInvoice, gasContractors, setValue]);

  // Auto-fill expense contractor (like in save-expenses-invoices-modal.tsx)
  useEffect(() => {
    if (watchedModuleType !== ModuleType.ProductionExpenses || !draftInvoice)
      return;
    const ef = draftInvoice.extractedFields;

    // First check if backend returned contractor ID (new or existing)
    if (ef.expenseContractorId) {
      setValue("expenseContractorId", ef.expenseContractorId);
      return;
    }

    // Try to match by NIP first (most reliable)
    if (expenseContractors.length > 0 && ef.sellerNip) {
      const normalizedInvoiceNip = ef.sellerNip.replace(/\D/g, "");
      const matchedByNip = expenseContractors.find(
        (c) => c.nip?.replace(/\D/g, "") === normalizedInvoiceNip,
      );
      if (matchedByNip) {
        setValue("expenseContractorId", matchedByNip.id);
        return;
      }
    }

    // Fallback: try to match by seller name (more flexible matching)
    if (expenseContractors.length > 0 && ef.sellerName) {
      const sellerNameLower = ef.sellerName.toLowerCase().trim();

      // First try exact match
      let matchedContractor = expenseContractors.find(
        (c) => c.name?.toLowerCase().trim() === sellerNameLower,
      );

      // If no exact match, try partial match
      if (!matchedContractor) {
        matchedContractor = expenseContractors.find((c) => {
          const contractorNameLower = c.name?.toLowerCase().trim() || "";
          return (
            contractorNameLower.includes(sellerNameLower) ||
            sellerNameLower.includes(contractorNameLower)
          );
        });
      }

      if (matchedContractor) {
        setValue("expenseContractorId", matchedContractor.id);
      }
    }
  }, [watchedModuleType, draftInvoice, expenseContractors, setValue]);

  // Auto-select expense type when contractor has only one expense type
  useEffect(() => {
    if (watchedModuleType !== ModuleType.ProductionExpenses) return;

    const selectedContractor = expenseContractors.find(
      (c) => c.id === watchedExpenseContractorId,
    );

    const currentValue = watch("expenseTypeId");

    if (selectedContractor && selectedContractor.expenseTypes.length === 1) {
      // Auto-select if contractor has exactly one expense type
      if (
        !currentValue ||
        !selectedContractor.expenseTypes.some((t) => t.id === currentValue)
      ) {
        setValue("expenseTypeId", selectedContractor.expenseTypes[0].id);
      }
    } else if (
      selectedContractor &&
      selectedContractor.expenseTypes.length > 1
    ) {
      // If contractor has multiple expense types, but current value is empty or not valid for this contractor
      if (
        !currentValue ||
        !selectedContractor.expenseTypes.some((t) => t.id === currentValue)
      ) {
        // Don't auto-select, but leave field empty so user can choose
        // (This handles the case where we want to show filtered list but not auto-select)
        setValue("expenseTypeId", "");
      }
    } else if (
      selectedContractor &&
      selectedContractor.expenseTypes.length === 0
    ) {
      // Clear the field if contractor has no expense types (user will need to select from all types)
      setValue("expenseTypeId", "");
    }
  }, [
    watchedExpenseContractorId,
    expenseContractors,
    watchedModuleType,
    setValue,
    watch,
  ]);

  // Auto-fill slaughterhouse (like in save-sales-invoices-modal.tsx)
  useEffect(() => {
    if (watchedModuleType !== ModuleType.Sales || !draftInvoice) return;
    const ef = draftInvoice.extractedFields;

    // First check if backend returned slaughterhouse ID (new or existing)
    if (ef.slaughterhouseId) {
      setValue("saleSlaughterhouseId", ef.slaughterhouseId);
      return;
    }

    // Try to match by buyer name (for sales, buyer is slaughterhouse)
    if (slaughterhouses.length > 0 && ef.buyerName) {
      const matchedSlaughterhouse = slaughterhouses.find(
        (s) => s.name.toLowerCase() === ef.buyerName?.toLowerCase(),
      );
      if (matchedSlaughterhouse) {
        setValue("saleSlaughterhouseId", matchedSlaughterhouse.id);
      }
    }
  }, [watchedModuleType, draftInvoice, slaughterhouses, setValue, savedCount]);

  // Auto-set payment date when payment status changes to paid
  useEffect(() => {
    // Skip auto-set during form reset to prevent overwriting values from new invoice
    if (isResettingRef.current) return;

    const isPaidStatus =
      watchedPaymentStatus === KSeFPaymentStatus.PaidCash ||
      watchedPaymentStatus === KSeFPaymentStatus.PaidTransfer;

    const currentPaymentDate = watch("paymentDate");

    if (isPaidStatus) {
      // For cash payment, use invoice date; for transfer, use today's date
      const newDate =
        watchedPaymentStatus === KSeFPaymentStatus.PaidCash &&
        watchedInvoiceDate
          ? watchedInvoiceDate
          : new Date().toISOString().split("T")[0];

      // Update date if it's empty or different from what it should be
      if (!currentPaymentDate || currentPaymentDate !== newDate) {
        setValue("paymentDate", newDate);
      }
    } else if (!isPaidStatus) {
      // Clear payment date if status is not paid
      setValue("paymentDate", "");
    }
  }, [watchedPaymentStatus, watchedInvoiceDate, setValue, watch]);

  // Fetch module-specific data
  useEffect(() => {
    if (!open) return;

    fetchFarms();

    // Feed names
    handleApiResponse(
      () => FeedsService.getFeedsNames(),
      (data) => setFeedNames(data.responseData?.fields ?? []),
      () => setFeedNames([]),
    );

    // Gas contractors
    handleApiResponse(
      () => GasService.getGasContractors(),
      (data) => setGasContractors(data.responseData?.contractors ?? []),
      () => setGasContractors([]),
    );

    // Expense contractors
    handleApiResponse(
      () => ExpensesService.getExpensesContractors({}),
      (data) => setExpenseContractors(data.responseData?.contractors ?? []),
      () => setExpenseContractors([]),
    );

    // Expense types
    handleApiResponse(
      () => ExpensesService.getExpensesTypes(),
      (data) => setExpenseTypes(data.responseData?.types ?? []),
      () => setExpenseTypes([]),
    );

    // Slaughterhouses
    handleApiResponse(
      () => SlaughterhousesService.getAllSlaughterhouses(),
      (data) => setSlaughterhouses(data.responseData?.items ?? []),
      () => setSlaughterhouses([]),
    );

    // Users
    handleApiResponse(
      () => UsersService.getUsers({ page: 0, pageSize: 100 }),
      (data) => setUsers(data.responseData?.items ?? []),
      () => setUsers([]),
    );
  }, [open, fetchFarms, savedCount]);

  // Reset saved count when modal closes
  useEffect(() => {
    if (!open) {
      setSavedCount(0);
    }
  }, [open]);

  useEffect(() => {
    if (!open) return;

    if (draftInvoices.length === 0) {
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
      const ef = currentDraft.extractedFields;
      const moduleType = (ef.moduleType as ModuleType) || ModuleType.None;

      // Set flag to prevent payment date effect from running during reset
      isResettingRef.current = true;

      // Auto-set payment date based on payment status
      let paymentDateValue = ef.paymentDate || "";
      if (ef.paymentStatus === "PaidCash" && ef.invoiceDate) {
        // For cash payment, use invoice date
        paymentDateValue = ef.invoiceDate;
      } else if (ef.paymentStatus === "PaidTransfer" && !ef.paymentDate) {
        // For transfer without date, use today
        paymentDateValue = new Date().toISOString().split("T")[0];
      }

      reset({
        invoiceNumber: ef.invoiceNumber || "",
        invoiceDate: ef.invoiceDate || "",
        dueDate: ef.dueDate || "",
        sellerName: ef.sellerName || "",
        sellerNip: ef.sellerNip || "",
        buyerName: ef.buyerName || "",
        buyerNip: ef.buyerNip || "",
        grossAmount: ef.grossAmount != null ? String(ef.grossAmount) : "",
        netAmount: ef.netAmount != null ? String(ef.netAmount) : "",
        vatAmount: ef.vatAmount != null ? String(ef.vatAmount) : "",
        documentType: InvoiceDocumentType.Vat,
        status: KSeFInvoiceStatus.Accepted,
        vatDeductionType: VatDeductionType.Full,
        moduleType: moduleType,
        paymentStatus:
          (ef.paymentStatus as KSeFPaymentStatus) || KSeFPaymentStatus.Unpaid,
        paymentDate: paymentDateValue,
        comment: "",
        assignedUserId: userData?.id || "",
        relatedInvoiceNumber: "",
        // Module-specific fields from AI extraction
        feedFarmId: moduleType === ModuleType.Feeds ? ef.farmId || "" : "",
        feedCycleId: moduleType === ModuleType.Feeds ? ef.cycleId || "" : "",
        feedHenhouseId:
          moduleType === ModuleType.Feeds ? ef.henhouseId || "" : "",
        feedBankAccountNumber:
          moduleType === ModuleType.Feeds ? ef.bankAccountNumber || "" : "",
        feedVendorName:
          moduleType === ModuleType.Feeds ? ef.sellerName || "" : "",
        feedItemName:
          moduleType === ModuleType.Feeds ? ef.feedItemName || "" : "",
        feedQuantity:
          moduleType === ModuleType.Feeds ? String(ef.feedQuantity || 0) : 0,
        feedUnitPrice:
          moduleType === ModuleType.Feeds ? String(ef.feedUnitPrice || 0) : 0,
        gasFarmId: moduleType === ModuleType.Gas ? ef.farmId || "" : "",
        gasCycleId: moduleType === ModuleType.Gas ? ef.cycleId || "" : "",
        gasContractorId:
          moduleType === ModuleType.Gas ? ef.gasContractorId || "" : "",
        gasUnitPrice:
          moduleType === ModuleType.Gas ? String(ef.gasUnitPrice || 0) : 0,
        gasQuantity:
          moduleType === ModuleType.Gas ? String(ef.gasQuantity || 0) : 0,
        expenseFarmId:
          moduleType === ModuleType.ProductionExpenses ? ef.farmId || "" : "",
        expenseCycleId:
          moduleType === ModuleType.ProductionExpenses ? ef.cycleId || "" : "",
        expenseContractorId:
          moduleType === ModuleType.ProductionExpenses
            ? ef.expenseContractorId || ""
            : "",
        expenseTypeId: "",
        saleFarmId: moduleType === ModuleType.Sales ? ef.farmId || "" : "",
        saleCycleId: moduleType === ModuleType.Sales ? ef.cycleId || "" : "",
        saleSlaughterhouseId:
          moduleType === ModuleType.Sales ? ef.slaughterhouseId || "" : "",
      });

      // Reset flag after form reset completes
      setTimeout(() => {
        isResettingRef.current = false;
      }, 0);
    }
  }, [currentIndex, draftInvoices, reset, open, handleClose, userData?.id]);

  // Auto-calculate amounts for Feeds module based on quantity and unit price
  useEffect(() => {
    if (
      watchedModuleType === ModuleType.Feeds &&
      watchedFeedQuantity &&
      watchedFeedUnitPrice
    ) {
      // Parse values that might be strings with commas/dots
      const parsedQty =
        typeof watchedFeedQuantity === "string"
          ? parseFormattedNumber(watchedFeedQuantity)
          : String(watchedFeedQuantity);
      const parsedPrice =
        typeof watchedFeedUnitPrice === "string"
          ? parseFormattedNumber(watchedFeedUnitPrice)
          : String(watchedFeedUnitPrice);

      if (!parsedQty || !parsedPrice) return;

      const quantity = Number(parsedQty);
      const unitPrice = Number(parsedPrice);

      if (
        !isNaN(quantity) &&
        !isNaN(unitPrice) &&
        quantity > 0 &&
        unitPrice > 0
      ) {
        // Calculate net amount (quantity * unit price)
        const netAmount = quantity * unitPrice;

        // Get current VAT amount from form (don't auto-calculate)
        const currentVatAmount = watch("vatAmount");
        const vatAmount = currentVatAmount
          ? Number(parseFormattedNumber(String(currentVatAmount)))
          : 0;

        // Calculate gross amount (net + VAT)
        const grossAmount = netAmount + vatAmount;

        // Round to 2 decimal places and convert to string
        setValue("netAmount", String(Math.round(netAmount * 100) / 100));
        setValue("grossAmount", String(Math.round(grossAmount * 100) / 100));
      }
    }
  }, [
    watchedModuleType,
    watchedFeedQuantity,
    watchedFeedUnitPrice,
    setValue,
    watch,
  ]);

  // Recalculate gross amount when net amount or VAT amount changes (for all module types)
  useEffect(() => {
    if (watchedNetAmount !== undefined && watchedVatAmount !== undefined) {
      const netAmount = Number(
        parseFormattedNumber(String(watchedNetAmount || 0)),
      );
      const vatAmount = Number(
        parseFormattedNumber(String(watchedVatAmount || 0)),
      );

      if (
        !isNaN(netAmount) &&
        !isNaN(vatAmount) &&
        netAmount >= 0 &&
        vatAmount >= 0
      ) {
        // Calculate gross amount (net + VAT)
        const grossAmount = netAmount + vatAmount;

        // Only update if the calculated gross amount differs from current
        const currentGross = Number(
          parseFormattedNumber(String(watchedGrossAmount || 0)),
        );
        if (Math.abs(grossAmount - currentGross) > 0.01) {
          setValue("grossAmount", String(Math.round(grossAmount * 100) / 100));
        }
      }
    }
  }, [watchedNetAmount, watchedVatAmount, watchedGrossAmount, setValue]);

  const handleSave = async (formData: SaveAccountingInvoiceFormData) => {
    if (!draftInvoice) return;

    // Helper function to convert string | number to number
    const toNumber = (value: string | number): number => {
      if (typeof value === "number") return value;
      const parsed = parseFormattedNumber(value);
      return parsed ? Number(parsed) : 0;
    };

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
          quantity: toNumber(formData.feedQuantity),
          unitPrice: toNumber(formData.feedUnitPrice),
        };
        break;
      case ModuleType.Gas:
        gasData = {
          farmId: formData.gasFarmId,
          cycleId: formData.gasCycleId || undefined,
          contractorId: formData.gasContractorId || undefined,
          unitPrice: toNumber(formData.gasUnitPrice),
          quantity: toNumber(formData.gasQuantity),
        };
        break;
      case ModuleType.ProductionExpenses:
        expenseData = {
          farmId: formData.expenseFarmId,
          cycleId: formData.expenseCycleId,
          expenseContractorId: formData.expenseContractorId || undefined,
          expenseTypeId: formData.expenseTypeId,
          contractorName: formData.sellerName,
          contractorNip: formData.sellerNip,
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
          grossAmount: toNumber(formData.grossAmount),
          netAmount: toNumber(formData.netAmount),
          vatAmount: toNumber(formData.vatAmount),
          invoiceType: draftInvoice.extractedFields.invoiceType,
          documentType: formData.documentType,
          status: formData.status,
          vatDeductionType: formData.vatDeductionType,
          moduleType: formData.moduleType,
          paymentStatus: formData.paymentStatus,
          paymentDate: formData.paymentDate || undefined,
          comment: formData.comment || undefined,
          assignedUserId: formData.assignedUserId || undefined,
          relatedInvoiceNumber: formData.relatedInvoiceNumber || undefined,
          feedData,
          gasData,
          expenseData,
          saleData,
        }),
      () => {
        toast.success(`Pomyślnie zapisano fakturę: ${formData.invoiceNumber}`);
        setSavedCount((prev) => prev + 1); // Wymuś odświeżenie kontrahentów
        onSave(draftInvoice);
      },
      undefined,
      "Wystąpił błąd podczas zapisywania faktury",
    );
    setLoading(false);
  };

  const renderPreview = () => {
    const file = previewFile || draftInvoice?.fileUrl;
    if (!file) return <Typography>Brak podglądu</Typography>;

    return (
      <FilePreview
        file={file}
        maxHeight={isLg ? 1400 : isMd ? 1100 : 800}
        showPreviewButton={true}
      />
    );
  };

  if (!draftInvoice) {
    return null;
  }

  const invoiceDetailsSection = (
    <>
      <Grid size={12}>
        <Divider>
          <Typography variant="caption">Dane faktury</Typography>
        </Divider>
      </Grid>

      <Grid size={{ xs: 12, sm: 4 }}>
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

      <Grid size={{ xs: 12, sm: 4 }}>
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
                field.onChange(date ? dayjs(date).format("YYYY-MM-DD") : "")
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
        <Controller
          name="dueDate"
          control={control}
          rules={{
            required: "Termin płatności jest wymagany",
            validate: (value) => {
              if (!value) {
                return "Termin płatności jest wymagany - uzupełnij to pole";
              }
              return true;
            },
          }}
          render={({ field }) => (
            <DatePicker
              label="Termin płatności"
              format="DD.MM.YYYY"
              value={field.value ? dayjs(field.value) : null}
              onChange={(date) =>
                field.onChange(date ? dayjs(date).format("YYYY-MM-DD") : "")
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
        <Controller
          name="netAmount"
          control={control}
          rules={{
            required: "Kwota netto jest wymagana",
            validate: (value) => {
              const parsed = parseFormattedNumber(String(value || ""));
              const num = Number(parsed);
              return (
                (!isNaN(num) && num >= 0) ||
                "Kwota musi być liczbą większą lub równą 0"
              );
            },
          }}
          render={({ field }) => (
            <TextField
              label="Kwota netto [zł]"
              value={formatNumberWithSpaces(field.value || "")}
              onChange={(e) => {
                field.onChange(e.target.value);
              }}
              error={!!errors.netAmount}
              helperText={errors.netAmount?.message}
              fullWidth
              required
              slotProps={{
                inputLabel: { shrink: true },
              }}
            />
          )}
        />
      </Grid>

      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="vatAmount"
          control={control}
          rules={{
            required: "Kwota VAT jest wymagana",
            validate: (value) => {
              const parsed = parseFormattedNumber(String(value || ""));
              const num = Number(parsed);
              return (
                (!isNaN(num) && num >= 0) ||
                "Kwota musi być liczbą większą lub równą 0"
              );
            },
          }}
          render={({ field }) => (
            <TextField
              label="Kwota VAT [zł]"
              value={formatNumberWithSpaces(field.value || "")}
              onChange={(e) => {
                field.onChange(e.target.value);
              }}
              error={!!errors.vatAmount}
              helperText={errors.vatAmount?.message}
              fullWidth
              required
              slotProps={{
                inputLabel: { shrink: true },
              }}
            />
          )}
        />
      </Grid>

      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="grossAmount"
          control={control}
          rules={{
            required: "Kwota brutto jest wymagana",
            validate: (value) => {
              const parsed = parseFormattedNumber(String(value || ""));
              const num = Number(parsed);
              return (
                (!isNaN(num) && num >= 0) ||
                "Kwota musi być liczbą większą lub równą 0"
              );
            },
          }}
          render={({ field }) => (
            <TextField
              label="Kwota brutto [zł]"
              value={formatNumberWithSpaces(field.value || "")}
              onChange={(e) => {
                field.onChange(e.target.value);
              }}
              error={!!errors.grossAmount}
              helperText={errors.grossAmount?.message}
              fullWidth
              required
              slotProps={{
                inputLabel: { shrink: true },
              }}
            />
          )}
        />
      </Grid>
    </>
  );

  const classificationSection = (
    <>
      <Grid size={12}>
        <Divider>
          <Typography variant="caption">Klasyfikacja faktury</Typography>
        </Divider>
      </Grid>

      <Grid size={{ xs: 12, sm: 6 }}>
        <FormControl fullWidth>
          <InputLabel id="document-type-label">Typ dokumentu</InputLabel>
          <Select
            labelId="document-type-label"
            label="Typ dokumentu"
            defaultValue={InvoiceDocumentType.Vat}
            {...register("documentType")}
          >
            {Object.entries(InvoiceDocumentTypeLabels).map(([key, label]) => (
              <MenuItem key={key} value={key}>
                {label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Grid>

      <Grid size={{ xs: 12, sm: 6 }}>
        <FormControl fullWidth>
          <InputLabel id="vat-deduction-label">Odliczenie VAT</InputLabel>
          <Select
            labelId="vat-deduction-label"
            label="Odliczenie VAT"
            defaultValue={VatDeductionType.Full}
            {...register("vatDeductionType")}
          >
            {Object.entries(VatDeductionTypeLabels).map(([key, label]) => (
              <MenuItem key={key} value={key}>
                {label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Grid>

      <Grid size={{ xs: 12, sm: 6 }}>
        <TextField
          label="Powiązanie faktury"
          fullWidth
          placeholder="Numer powiązanej faktury"
          slotProps={{ inputLabel: { shrink: true } }}
          {...register("relatedInvoiceNumber")}
        />
      </Grid>
    </>
  );

  const statusSection = (
    <>
      <Grid size={12}>
        <Divider>
          <Typography variant="caption">Status faktury</Typography>
        </Divider>
      </Grid>

      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="status"
          control={control}
          defaultValue={KSeFInvoiceStatus.Accepted}
          render={({ field }) => (
            <FormControl fullWidth>
              <InputLabel id="invoice-status-label">Status faktury</InputLabel>
              <Select
                {...field}
                labelId="invoice-status-label"
                label="Status faktury"
              >
                {Object.entries(KSeFInvoiceStatusLabels).map(([key, label]) => (
                  <MenuItem key={key} value={key}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}
        />
      </Grid>

      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="paymentStatus"
          control={control}
          defaultValue={KSeFPaymentStatus.Unpaid}
          render={({ field }) => (
            <FormControl fullWidth>
              <InputLabel id="payment-status-label">
                Status płatności
              </InputLabel>
              <Select
                {...field}
                labelId="payment-status-label"
                label="Status płatności"
              >
                {Object.entries(KSeFPaymentStatusLabels).map(([key, label]) => (
                  <MenuItem key={key} value={key}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}
        />
      </Grid>

      {(watchedPaymentStatus === KSeFPaymentStatus.PaidCash ||
        watchedPaymentStatus === KSeFPaymentStatus.PaidTransfer) && (
        <Grid size={{ xs: 12, sm: 4 }}>
          <TextField
            fullWidth
            label="Data płatności"
            type="date"
            {...register("paymentDate")}
            InputLabelProps={{ shrink: true }}
            inputProps={{
              max: new Date().toISOString().split("T")[0],
            }}
          />
        </Grid>
      )}
    </>
  );

  const commentSection = (
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
  );

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="xl">
      <DialogTitle>
        Podgląd faktury i dane ({currentIndex + 1} / {draftInvoices.length})
      </DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent
          dividers
          sx={{
            p: 0,
            display: "flex",
            flexDirection: { xs: "column", lg: "row" },
            height: { lg: "70vh" },
          }}
        >
          <Grid container spacing={0} sx={{ height: "100%" }}>
            <Grid
              size={{ md: 12, lg: 7, xl: 8 }}
              sx={{
                height: { lg: "100%" },
                overflowY: { lg: "auto" },
                p: 2,
                borderRight: { lg: 1 },
                borderColor: { lg: "divider" },
              }}
            >
              <Typography variant="h6">Podgląd faktury</Typography>
              {renderPreview()}
            </Grid>

            <Grid
              size={{ md: 12, lg: 5, xl: 4 }}
              sx={{
                height: { lg: "100%" },
                overflowY: { lg: "auto" },
                p: 2,
              }}
            >
              <Grid container spacing={3} alignItems={"top"}>
                <Grid size={12}>
                  <Typography variant="h6">Dane zaczytane z faktury</Typography>
                  <Typography variant="body2" color="text.secondary">
                    Sprawdź i popraw dane, jeśli AI zaczytało je niepoprawnie
                  </Typography>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <Controller
                    name="moduleType"
                    control={control}
                    rules={{
                      required: "Moduł jest wymagany",
                      validate: (value) =>
                        value !== ModuleType.None || "Wybierz moduł",
                    }}
                    render={({ field }) => (
                      <FormControl
                        fullWidth
                        error={!!errors.moduleType}
                        required
                      >
                        <InputLabel id="module-type-label">Moduł</InputLabel>
                        <Select
                          labelId="module-type-label"
                          label="Moduł"
                          value={field.value || ""}
                          onChange={field.onChange}
                          onBlur={field.onBlur}
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
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <FormControl fullWidth>
                    <InputLabel id="assigned-user-label">
                      Przypisany pracownik
                    </InputLabel>
                    <Select
                      labelId="assigned-user-label"
                      label="Przypisany pracownik"
                      {...register("assignedUserId")}
                      value={watch("assignedUserId") || ""}
                    >
                      {users
                        .filter((user) => user.login !== "admin")
                        .map((user) => (
                          <MenuItem key={user.id} value={user.id}>
                            {user.name}
                          </MenuItem>
                        ))}
                    </Select>
                  </FormControl>
                </Grid>

                <Grid size={12}>
                  <Divider>
                    <Typography variant="caption">
                      {watchedModuleType === ModuleType.ProductionExpenses
                        ? "Sprzedawca / Kontrahent"
                        : watchedModuleType === ModuleType.Gas
                          ? "Sprzedawca / Dostawca gazu"
                          : watchedModuleType === ModuleType.Feeds
                            ? "Sprzedawca / Dostawca paszy"
                            : "Sprzedawca"}
                    </Typography>
                  </Divider>
                </Grid>

                {/* ProductionExpenses: show contractor and expense type in seller section, farm and cycle in buyer section */}
                {watchedModuleType === ModuleType.ProductionExpenses ? (
                  <>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Controller
                        name="expenseContractorId"
                        control={control}
                        rules={{
                          validate: (value) => {
                            if (
                              watchedModuleType !==
                              ModuleType.ProductionExpenses
                            )
                              return true;
                            return (
                              !!value ||
                              draftInvoice?.extractedFields
                                .isNewExpenseContractor ||
                              "Kontrahent jest wymagany"
                            );
                          },
                        }}
                        render={({ field: { onChange, value } }) => (
                          <Autocomplete
                            options={expenseContractors}
                            getOptionLabel={(option) => option.name || ""}
                            value={
                              expenseContractors.find((c) => c.id === value) ||
                              null
                            }
                            onChange={(_, newValue) => {
                              onChange(newValue?.id || "");
                              // Sync seller name with contractor
                              if (newValue) {
                                setValue("sellerName", newValue.name);
                                setValue("sellerNip", newValue.nip || "");
                              }
                              // Only clear expense type if the new contractor has different types
                              if (newValue) {
                                const currentExpenseTypeId =
                                  watch("expenseTypeId");
                                const hasCurrentType =
                                  newValue.expenseTypes.some(
                                    (t) => t.id === currentExpenseTypeId,
                                  );
                                if (!hasCurrentType) {
                                  setValue("expenseTypeId", "");
                                }
                              } else {
                                setValue("expenseTypeId", "");
                              }
                            }}
                            renderInput={(params) => (
                              <TextField
                                {...params}
                                label="Kontrahent (Sprzedawca)"
                                required
                                error={!!errors.expenseContractorId}
                                helperText={
                                  errors.expenseContractorId?.message ||
                                  (draftInvoice?.extractedFields
                                    .isNewExpenseContractor
                                    ? "Nowy kontrahent - zostanie utworzony przy zapisie"
                                    : undefined)
                                }
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
                                const selectedContractor =
                                  expenseContractors.find(
                                    (c) => c.id === watchedExpenseContractorId,
                                  );

                                // Jeśli kontrahent jest wybrany i ma przypisane typy wydatków, pokaż tylko te typy
                                if (
                                  selectedContractor &&
                                  selectedContractor.expenseTypes.length > 0
                                ) {
                                  return selectedContractor.expenseTypes.map(
                                    (type) => (
                                      <MenuItem key={type.id} value={type.id}>
                                        {type.name}
                                      </MenuItem>
                                    ),
                                  );
                                }

                                // Jeśli kontrahent nie jest wybrany lub nie ma przypisanych typów, pokaż wszystkie
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
                            {draftInvoice?.extractedFields
                              .isNewExpenseContractor && (
                              <FormHelperText>
                                Wybrany typ zostanie przypisany do nowego
                                kontrahenta
                              </FormHelperText>
                            )}
                          </FormControl>
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        label="NIP sprzedawcy"
                        value={watch("sellerNip") || ""}
                        fullWidth
                        slotProps={{ inputLabel: { shrink: true } }}
                        disabled
                      />
                    </Grid>
                    {/* Hidden fields for backend compatibility */}
                    <input type="hidden" {...register("sellerName")} />
                    <input type="hidden" {...register("sellerNip")} />
                  </>
                ) : watchedModuleType === ModuleType.Gas ? (
                  <>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Controller
                        name="gasContractorId"
                        control={control}
                        rules={{
                          required: false,
                        }}
                        render={({ field: { onChange, value } }) => (
                          <Autocomplete
                            options={gasContractors}
                            getOptionLabel={(option) => option.name || ""}
                            value={
                              gasContractors.find((c) => c.id === value) || null
                            }
                            onChange={(_, newValue) => {
                              onChange(newValue?.id || "");
                              // Sync seller name with contractor
                              if (newValue) {
                                setValue("sellerName", newValue.name);
                                setValue("sellerNip", newValue.nip || "");
                              }
                            }}
                            renderInput={(params) => (
                              <TextField
                                {...params}
                                label="Dostawca gazu (Sprzedawca)"
                              />
                            )}
                          />
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        label="NIP sprzedawcy"
                        value={watch("sellerNip") || ""}
                        fullWidth
                        slotProps={{ inputLabel: { shrink: true } }}
                        disabled
                      />
                    </Grid>
                    {/* Hidden fields for backend compatibility */}
                    <input type="hidden" {...register("sellerName")} />
                    <input type="hidden" {...register("sellerNip")} />
                  </>
                ) : (
                  <>
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
                  </>
                )}

                <Grid size={12}>
                  <Divider>
                    <Typography variant="caption">
                      {watchedModuleType === ModuleType.Sales
                        ? "Nabywca / Ubojnia"
                        : watchedModuleType === ModuleType.ProductionExpenses
                          ? "Nabywca / Ferma i Cykl"
                          : "Nabywca"}
                    </Typography>
                  </Divider>
                </Grid>

                {/* Sales: show slaughterhouse in buyer section */}
                {watchedModuleType === ModuleType.Sales ? (
                  <>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Controller
                        name="saleSlaughterhouseId"
                        control={control}
                        rules={{
                          validate: (value) => {
                            if (watchedModuleType !== ModuleType.Sales)
                              return true;
                            return (
                              !!value ||
                              draftInvoice?.extractedFields
                                .isNewSlaughterhouse ||
                              "Ubojnia jest wymagana"
                            );
                          },
                        }}
                        render={({ field: { onChange, value } }) => (
                          <Autocomplete
                            options={slaughterhouses}
                            getOptionLabel={(option) => option.name || ""}
                            value={
                              slaughterhouses.find((s) => s.id === value) ||
                              null
                            }
                            onChange={(_, newValue) => {
                              onChange(newValue?.id || "");
                              // Sync buyer name with slaughterhouse
                              if (newValue) {
                                setValue("buyerName", newValue.name);
                                setValue("buyerNip", newValue.nip || "");
                              }
                            }}
                            renderInput={(params) => (
                              <TextField
                                {...params}
                                label="Ubojnia (Nabywca)"
                                required
                                error={!!errors.saleSlaughterhouseId}
                                helperText={
                                  errors.saleSlaughterhouseId?.message ||
                                  (draftInvoice?.extractedFields
                                    .isNewSlaughterhouse
                                    ? "Nowa ubojnia - zostanie utworzona przy zapisie"
                                    : undefined)
                                }
                              />
                            )}
                          />
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <TextField
                        label="NIP nabywcy"
                        value={watch("buyerNip") || ""}
                        fullWidth
                        slotProps={{ inputLabel: { shrink: true } }}
                        disabled
                      />
                    </Grid>
                    {/* Hidden fields for backend compatibility */}
                    <input type="hidden" {...register("buyerName")} />
                    <input type="hidden" {...register("buyerNip")} />
                  </>
                ) : watchedModuleType === ModuleType.ProductionExpenses ? (
                  <>
                    {/* For ProductionExpenses: show farm and cycle in buyer section */}
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
                  </>
                ) : (
                  <>
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
                  </>
                )}

                {watchedModuleType === ModuleType.Feeds ? (
                  <>
                    {classificationSection}
                    {statusSection}
                    {invoiceDetailsSection}
                  </>
                ) : (
                  <>
                    {invoiceDetailsSection}
                    {classificationSection}
                    {statusSection}
                    {commentSection}
                  </>
                )}

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
                      <Controller
                        name="feedItemName"
                        control={control}
                        rules={{
                          validate: (value) => {
                            if (watchedModuleType !== ModuleType.Feeds)
                              return true;
                            return !!value || "Nazwa paszy jest wymagana";
                          },
                        }}
                        render={({ field: { onChange, value, ref } }) => (
                          <Autocomplete
                            freeSolo
                            options={feedNames.map((f) => f.name)}
                            value={value || ""}
                            onChange={(_, newValue) => onChange(newValue || "")}
                            onInputChange={(_, newInputValue) => {
                              // For freeSolo, we might want to update value on input change if it doesn't match option
                              // But here we want the value to be the string
                              onChange(newInputValue);
                            }}
                            renderInput={(params) => (
                              <TextField
                                {...params}
                                label="Nazwa paszy"
                                inputRef={ref}
                                required
                                error={!!errors.feedItemName}
                                helperText={errors.feedItemName?.message}
                              />
                            )}
                          />
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Controller
                        name="feedQuantity"
                        control={control}
                        rules={{
                          required: "Ilość jest wymagana",
                          validate: (value) => {
                            const parsed = parseFormattedNumber(
                              String(value || ""),
                            );
                            const num = Number(parsed);
                            return (
                              (!isNaN(num) && num > 0) ||
                              "Ilość musi być liczbą większą od 0"
                            );
                          },
                        }}
                        render={({ field }) => (
                          <TextField
                            label="Ilość [t]"
                            value={field.value || ""}
                            onChange={(e) => {
                              field.onChange(e.target.value);
                            }}
                            error={!!errors.feedQuantity}
                            helperText={errors.feedQuantity?.message}
                            fullWidth
                            required
                            slotProps={{
                              inputLabel: { shrink: true },
                            }}
                          />
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Controller
                        name="feedUnitPrice"
                        control={control}
                        rules={{
                          required: "Cena jest wymagana",
                          validate: (value) => {
                            const parsed = parseFormattedNumber(
                              String(value || ""),
                            );
                            const num = Number(parsed);
                            return (
                              (!isNaN(num) && num > 0) ||
                              "Cena musi być liczbą większą od 0"
                            );
                          },
                        }}
                        render={({ field }) => (
                          <TextField
                            label="Cena jednostkowa [zł/t]"
                            value={field.value || ""}
                            onChange={(e) => {
                              field.onChange(e.target.value);
                            }}
                            error={!!errors.feedUnitPrice}
                            helperText={errors.feedUnitPrice?.message}
                            fullWidth
                            required
                            slotProps={{
                              inputLabel: { shrink: true },
                            }}
                          />
                        )}
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
                        required
                        slotProps={{ inputLabel: { shrink: true } }}
                        error={!!errors.feedBankAccountNumber}
                        helperText={errors.feedBankAccountNumber?.message}
                        {...register("feedBankAccountNumber", {
                          required: "Nr konta bankowego jest wymagany",
                        })}
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
                        label="Lokalizacja (Ferma)"
                        fullWidth
                        required
                        value={watchedGasFarmId || ""}
                        error={!!errors.gasFarmId}
                        helperText={errors.gasFarmId?.message}
                        {...register("gasFarmId", {
                          required: "Lokalizacja jest wymagana",
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
                        label="Cykl (dla kosztów produkcyjnych)"
                        fullWidth
                        disabled={!watchedGasFarmId}
                        value={watch("gasCycleId") || ""}
                        helperText="Opcjonalne - jeśli wybrane, zostanie utworzony wpis w kosztach produkcyjnych"
                        {...register("gasCycleId")}
                      >
                        {cycles.map((cycle) => (
                          <MenuItem key={cycle.id} value={cycle.id}>
                            {cycle.identifier}/{cycle.year}
                          </MenuItem>
                        ))}
                      </LoadingTextField>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Controller
                        name="gasQuantity"
                        control={control}
                        rules={{
                          required: "Ilość jest wymagana",
                          validate: (value) => {
                            const parsed = parseFormattedNumber(
                              String(value || ""),
                            );
                            const num = Number(parsed);
                            return (
                              (!isNaN(num) && num > 0) ||
                              "Ilość musi być liczbą większą od 0"
                            );
                          },
                        }}
                        render={({ field }) => (
                          <TextField
                            label="Ilość [l]"
                            value={field.value || ""}
                            onChange={(e) => {
                              field.onChange(e.target.value);
                            }}
                            error={!!errors.gasQuantity}
                            helperText={errors.gasQuantity?.message}
                            fullWidth
                            required
                            slotProps={{
                              inputLabel: { shrink: true },
                            }}
                          />
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <Controller
                        name="gasUnitPrice"
                        control={control}
                        rules={{
                          required: "Cena jest wymagana",
                          validate: (value) => {
                            const parsed = parseFormattedNumber(
                              String(value || ""),
                            );
                            const num = Number(parsed);
                            return (
                              (!isNaN(num) && num > 0) ||
                              "Cena musi być liczbą większą od 0"
                            );
                          },
                        }}
                        render={({ field }) => (
                          <TextField
                            label="Cena jednostkowa [zł/l]"
                            value={field.value || ""}
                            onChange={(e) => {
                              field.onChange(e.target.value);
                            }}
                            error={!!errors.gasUnitPrice}
                            helperText={errors.gasUnitPrice?.message}
                            fullWidth
                            required
                            slotProps={{
                              inputLabel: { shrink: true },
                            }}
                          />
                        )}
                      />
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
                        label="Lokalizacja (Ferma)"
                        fullWidth
                        required
                        value={watchedSaleFarmId || ""}
                        error={!!errors.saleFarmId}
                        helperText={errors.saleFarmId?.message}
                        {...register("saleFarmId", {
                          required: "Lokalizacja jest wymagana",
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
                  </>
                )}

                {/* Investments module - only location, no cycle */}
                {watchedModuleType === ModuleType.Investments && (
                  <>
                    <Grid size={{ xs: 12, sm: 6 }}>
                      <LoadingTextField
                        loading={loadingFarms}
                        select
                        label="Lokalizacja (Ferma)"
                        fullWidth
                        value={watch("feedFarmId") || ""}
                        {...register("feedFarmId")}
                      >
                        {farms.map((farm) => (
                          <MenuItem key={farm.id} value={farm.id}>
                            {farm.name}
                          </MenuItem>
                        ))}
                      </LoadingTextField>
                    </Grid>
                  </>
                )}
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
                Math.min(draftInvoices.length - 1, prev + 1),
              )
            }
            disabled={currentIndex === draftInvoices.length - 1}
          >
            Następny
          </Button>

          <Button onClick={handleClose} disabled={loading}>
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
