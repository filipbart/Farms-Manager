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
  Box,
  CircularProgress,
  ButtonGroup,
  Autocomplete,
} from "@mui/material";
import { useState, useEffect, useCallback } from "react";
import { MdSave } from "react-icons/md";
import CheckIcon from "@mui/icons-material/Check";
import PauseIcon from "@mui/icons-material/Pause";
import CloseIcon from "@mui/icons-material/Close";
import DownloadIcon from "@mui/icons-material/Download";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import DeleteIcon from "@mui/icons-material/Delete";
import { AccountingService } from "../../../services/accounting-service";
import { FarmsService } from "../../../services/farms-service";
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
import type {
  KSeFInvoiceDetails,
  KSeFInvoiceListModel,
} from "../../../models/accounting/ksef-invoice";
import type FarmRowModel from "../../../models/farms/farm-row-model";
import type CycleDto from "../../../models/farms/latest-cycle";
import { UsersService } from "../../../services/users-service";
import type { UserListModel } from "../../../models/users/users";
import { useAuth } from "../../../auth/useAuth";
import { FeedsService } from "../../../services/feeds-service";
import type { FeedsNamesRow } from "../../../models/feeds/feeds-names";
import { GasService } from "../../../services/gas-service";
import type { GasContractorRow } from "../../../models/gas/gas-contractors";
import { ExpensesService } from "../../../services/expenses-service";
import type { ExpenseContractorRow } from "../../../models/expenses/expenses-contractors";
import type { ExpenseTypeRow } from "../../../models/expenses/expenses-types";
import { SlaughterhousesService } from "../../../services/slaughterhouses-service";
import type { SlaughterhouseRowModel } from "../../../models/slaughterhouses/slaughterhouse-row-model";
import ConfirmDialog from "../../common/confirm-dialog";
import {
  type InvoiceFormData,
  getFarmIdForModule,
  getCycleIdForModule,
} from "./types/invoice-form-types";
import { useInvoiceAttachments } from "./hooks/useInvoiceAttachments";

interface NonKSeFInvoiceDetailsModalProps {
  open: boolean;
  onClose: () => void;
  onSave?: () => void;
  invoice: KSeFInvoiceListModel | null;
  // Sequential processing props
  sequentialMode?: boolean;
  currentIndex?: number;
  totalCount?: number;
  onNext?: () => void;
  onPrevious?: () => void;
  onExitSequential?: () => void;
}

const NonKSeFInvoiceDetailsModal: React.FC<NonKSeFInvoiceDetailsModalProps> = ({
  open,
  onClose,
  onSave,
  invoice,
  sequentialMode = false,
  currentIndex = 0,
  totalCount = 0,
  onNext,
  onPrevious,
  onExitSequential,
}) => {
  const theme = useTheme();
  const isMd = useMediaQuery(theme.breakpoints.up("md"));
  const isLg = useMediaQuery(theme.breakpoints.up("lg"));
  const { userData } = useAuth();

  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [details, setDetails] = useState<KSeFInvoiceDetails | null>(null);
  const [previewFile, setPreviewFile] = useState<File | null>(null);

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

  // Attachments hook
  const {
    attachments,
    attachmentsLoading,
    uploadingAttachment,
    fileInputRef,
    attachmentToDelete,
    setAttachmentToDelete,
    handleAttachmentUpload,
    handleAttachmentDownload,
    handleAttachmentDelete,
    confirmAttachmentDelete,
  } = useInvoiceAttachments({ invoiceId: details?.id });

  // Hold modal state
  const [showHoldModal, setShowHoldModal] = useState(false);
  const [holdUserId, setHoldUserId] = useState<string>("");
  const [holdSaving, setHoldSaving] = useState(false);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    watch,
    setValue,
  } = useForm<InvoiceFormData>({
    mode: "onSubmit",
  });

  const watchedModuleType = watch("moduleType");
  const watchedFeedFarmId = watch("feedFarmId");
  const watchedGasFarmId = watch("gasFarmId");
  const watchedExpenseFarmId = watch("expenseFarmId");
  const watchedSaleFarmId = watch("saleFarmId");
  const watchedExpenseContractorId = watch("expenseContractorId");
  const watchedPaymentStatus = watch("paymentStatus");
  const watchedInvoiceDate = watch("invoiceDate");
  const watchedSellerNip = watch("sellerNip");
  const watchedStatus = watch("status");
  const isPaidPaymentStatus =
    watchedPaymentStatus === KSeFPaymentStatus.PaidCash ||
    watchedPaymentStatus === KSeFPaymentStatus.PaidTransfer;

  // Fetch invoice details
  useEffect(() => {
    const fetchDetails = async () => {
      if (!invoice) return;
      setLoading(true);
      try {
        await handleApiResponse(
          () => AccountingService.getKSeFInvoiceDetails(invoice.id),
          (data) => {
            if (data.responseData) {
              setDetails(data.responseData);
            }
          },
          undefined,
          "Błąd podczas pobierania szczegółów faktury",
        );
      } catch {
        toast.error("Błąd podczas pobierania szczegółów faktury");
      } finally {
        setLoading(false);
      }
    };

    if (open && invoice) {
      fetchDetails();
    } else {
      setDetails(null);
    }
  }, [open, invoice]);

  // Fetch file blob for preview
  useEffect(() => {
    let active = true;
    const fetchPreview = async () => {
      setPreviewFile(null);
      if (!details?.filePath) {
        return;
      }

      try {
        let fileType: FileType;
        if (details.filePath.startsWith("FeedDeliveryInvoice/")) {
          fileType = FileType.FeedDeliveryInvoice;
        } else if (details.filePath.startsWith("GasDelivery/")) {
          fileType = FileType.GasDelivery;
        } else if (details.filePath.startsWith("ExpenseProduction/")) {
          fileType = FileType.ExpenseProduction;
        } else if (details.filePath.startsWith("SalesInvoices/")) {
          fileType = FileType.SalesInvoices;
        } else {
          fileType = FileType.AccountingInvoice;
        }

        const blob = await FilesService.getFile(details.filePath, fileType);

        if (active && blob) {
          const fileName = details.filePath.split("/").pop() || "invoice";
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
  }, [details?.filePath]);

  // Fetch farms
  const fetchFarms = useCallback(async () => {
    setLoadingFarms(true);
    await handleApiResponse(
      () => FarmsService.getFarmsAsync(),
      (data) => setFarms(data.responseData?.items || []),
      () => setFarms([]),
    );
    setLoadingFarms(false);
  }, []);

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
      if (farm?.activeCycle) {
        setValue("feedCycleId", farm.activeCycle.id);
      }
    }
  }, [watchedFeedFarmId, farms, fetchCycles, setValue]);

  // Auto-set payment date when payment status changes to paid
  useEffect(() => {
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
      setValue("paymentDate", "");
    }
  }, [watchedPaymentStatus, watchedInvoiceDate, setValue, watch]);

  // Fetch module-specific data
  useEffect(() => {
    fetchFarms();
    // Fetch feed names for autocomplete
    handleApiResponse(
      () => FeedsService.getFeedsNames(),
      (data) => setFeedNames(data.responseData?.fields || []),
      () => setFeedNames([]),
    );
    // Fetch gas contractors
    handleApiResponse(
      () => GasService.getGasContractors(),
      (data) => setGasContractors(data.responseData?.contractors || []),
      () => setGasContractors([]),
    );
    // Fetch expense contractors
    handleApiResponse(
      () => ExpensesService.getExpensesContractors({}),
      (data) => setExpenseContractors(data.responseData?.contractors || []),
      () => setExpenseContractors([]),
    );
    // Fetch expense types
    handleApiResponse(
      () => ExpensesService.getExpensesTypes(),
      (data) => setExpenseTypes(data.responseData?.types || []),
      () => setExpenseTypes([]),
    );
    // Fetch slaughterhouses
    handleApiResponse(
      () => SlaughterhousesService.getAllSlaughterhouses(),
      (data) => setSlaughterhouses(data.responseData?.items || []),
      () => setSlaughterhouses([]),
    );
    // Fetch users
    handleApiResponse(
      () => UsersService.getUsers({ page: 0, pageSize: 100 }),
      (data) => setUsers(data.responseData?.items ?? []),
      () => setUsers([]),
    );
  }, [fetchFarms]);

  useEffect(() => {
    if (watchedExpenseFarmId && farms.length > 0) {
      fetchCycles(watchedExpenseFarmId);
    }
  }, [watchedExpenseFarmId, farms, fetchCycles]);

  // Fetch cycles when sale farm changes
  useEffect(() => {
    if (watchedSaleFarmId && farms.length > 0) {
      fetchCycles(watchedSaleFarmId);
      const farm = farms.find((f) => f.id === watchedSaleFarmId);
      if (farm?.activeCycle) {
        setValue("saleCycleId", farm.activeCycle.id);
      }
    }
  }, [watchedSaleFarmId, farms, fetchCycles, setValue]);

  // Auto-select contractor by NIP for Production Expenses module when rejected or no contractor assigned
  useEffect(() => {
    if (
      watchedModuleType === ModuleType.ProductionExpenses &&
      watchedSellerNip &&
      expenseContractors.length > 0 &&
      (!watchedExpenseContractorId ||
        watchedStatus === KSeFInvoiceStatus.Rejected)
    ) {
      const matchingContractor = expenseContractors.find(
        (c) => c.nip === watchedSellerNip,
      );
      if (
        matchingContractor &&
        matchingContractor.id !== watchedExpenseContractorId
      ) {
        setValue("expenseContractorId", matchingContractor.id);
      }
    }
  }, [
    watchedModuleType,
    watchedSellerNip,
    watchedExpenseContractorId,
    watchedStatus,
    expenseContractors,
    setValue,
  ]);

  // Initialize form with invoice details
  useEffect(() => {
    if (!details) return;

    const moduleType = (details.moduleType as ModuleType) || ModuleType.None;

    reset({
      invoiceNumber: details.invoiceNumber || "",
      invoiceDate: details.invoiceDate
        ? dayjs(details.invoiceDate).format("YYYY-MM-DD")
        : "",
      dueDate: details.paymentDueDate
        ? dayjs(details.paymentDueDate).format("YYYY-MM-DD")
        : "",
      sellerName: details.sellerName || "",
      sellerNip: details.sellerNip || "",
      buyerName: details.buyerName || "",
      buyerNip: details.buyerNip || "",
      grossAmount: details.grossAmount || 0,
      netAmount: details.netAmount || 0,
      vatAmount: details.vatAmount || 0,
      documentType: InvoiceDocumentType.Vat,
      status: details.status || KSeFInvoiceStatus.New,
      vatDeductionType:
        (details.vatDeductionType as VatDeductionType) || VatDeductionType.Full,
      moduleType: moduleType,
      paymentStatus: details.paymentStatus || KSeFPaymentStatus.Unpaid,
      paymentDate: details.paymentDate
        ? dayjs(details.paymentDate).format("YYYY-MM-DD")
        : "",
      comment: details.comment || "",
      assignedUserId: details.assignedUserId || userData?.id || "",
      relatedInvoiceNumber: details.relatedInvoiceNumber || "",
      // Module-specific fields
      feedFarmId: moduleType === ModuleType.Feeds ? details.farmId || "" : "",
      feedCycleId: moduleType === ModuleType.Feeds ? details.cycleId || "" : "",
      feedHenhouseId:
        moduleType === ModuleType.Feeds ? details.feedHenhouseId || "" : "",
      feedBankAccountNumber:
        moduleType === ModuleType.Feeds
          ? details.feedBankAccountNumber || ""
          : "",
      feedVendorName:
        moduleType === ModuleType.Feeds
          ? details.feedVendorName || details.sellerName || ""
          : details.sellerName || "",
      feedItemName:
        moduleType === ModuleType.Feeds ? details.feedItemName || "" : "",
      feedQuantity:
        moduleType === ModuleType.Feeds
          ? String(details.feedQuantity || 0)
          : "",
      feedUnitPrice:
        moduleType === ModuleType.Feeds
          ? String(details.feedUnitPrice || 0)
          : "",
      gasFarmId: moduleType === ModuleType.Gas ? details.farmId || "" : "",
      gasContractorId:
        moduleType === ModuleType.Gas ? details.gasContractorId || "" : "",
      gasUnitPrice:
        moduleType === ModuleType.Gas ? String(details.gasUnitPrice || 0) : "",
      gasQuantity:
        moduleType === ModuleType.Gas ? String(details.gasQuantity || 0) : "",
      expenseFarmId:
        moduleType === ModuleType.ProductionExpenses
          ? details.farmId || ""
          : "",
      expenseCycleId:
        moduleType === ModuleType.ProductionExpenses
          ? details.cycleId || ""
          : "",
      expenseContractorId:
        moduleType === ModuleType.ProductionExpenses
          ? details.expenseContractorId || ""
          : "",
      expenseTypeId:
        moduleType === ModuleType.ProductionExpenses
          ? details.expenseTypeId || ""
          : "",
      saleFarmId: moduleType === ModuleType.Sales ? details.farmId || "" : "",
      saleCycleId: moduleType === ModuleType.Sales ? details.cycleId || "" : "",
      saleSlaughterhouseId:
        moduleType === ModuleType.Sales
          ? details.saleSlaughterhouseId || ""
          : "",
    });
  }, [details, reset, userData?.id]);

  // Handle status change
  const handleStatusChange = async (newStatus: KSeFInvoiceStatus) => {
    if (!details) return;
    setSaving(true);
    try {
      if (newStatus === KSeFInvoiceStatus.Rejected) {
        await handleApiResponse(
          () => AccountingService.rejectInvoice(details.id),
          () => {
            toast.success("Faktura została odrzucona");
            setDetails((prev) =>
              prev ? { ...prev, status: newStatus } : null,
            );
            onSave?.();
          },
          undefined,
          "Błąd podczas odrzucania faktury",
        );
      } else {
        await handleApiResponse(
          () =>
            AccountingService.updateInvoice(details.id, {
              status: newStatus,
            }),
          () => {
            toast.success("Status faktury został zmieniony");
            setDetails((prev) =>
              prev ? { ...prev, status: newStatus } : null,
            );
            onSave?.();
          },
          undefined,
          "Błąd podczas zmiany statusu",
        );
      }
    } finally {
      setSaving(false);
    }
  };

  // Handle hold invoice
  const handleHoldInvoice = async () => {
    if (!details || !holdUserId) {
      toast.warning("Wybierz pracownika");
      return;
    }
    setHoldSaving(true);
    try {
      const formData = watch();
      await handleApiResponse(
        () =>
          AccountingService.updateInvoice(details.id, {
            moduleType: formData.moduleType,
            farmId: getFarmIdForModule(formData) || null,
            cycleId: getCycleIdForModule(formData) || null,
            assignedUserId: holdUserId,
            vatDeductionType: formData.vatDeductionType,
            paymentStatus: formData.paymentStatus,
            paymentDate: formData.paymentDate || null,
            dueDate: formData.dueDate || null,
            comment: formData.comment,
            relatedInvoiceNumber: formData.relatedInvoiceNumber,
          }),
        () => {
          toast.success(
            "Zmiany zostały zapisane i faktura przypisana do pracownika",
          );
          setShowHoldModal(false);
          setHoldUserId("");
          onSave?.();
        },
        undefined,
        "Błąd podczas zapisywania zmian",
      );
    } finally {
      setHoldSaving(false);
    }
  };

  // Handle save
  const handleSave = async (formData: InvoiceFormData) => {
    if (!details) return;

    setSaving(true);
    try {
      // Build module-specific data
      // Update invoice with all data
      await handleApiResponse(
        () =>
          AccountingService.updateInvoice(details.id, {
            invoiceNumber: formData.invoiceNumber,
            invoiceDate: formData.invoiceDate,
            dueDate: formData.dueDate || null,
            sellerName: formData.sellerName,
            sellerNip: formData.sellerNip,
            buyerName: formData.buyerName,
            buyerNip: formData.buyerNip,
            grossAmount: formData.grossAmount,
            netAmount: formData.netAmount,
            vatAmount: formData.vatAmount,
            documentType: formData.documentType,
            vatDeductionType: formData.vatDeductionType,
            moduleType: formData.moduleType,
            paymentStatus: formData.paymentStatus,
            paymentDate: formData.paymentDate || null,
            comment: formData.comment,
            assignedUserId: formData.assignedUserId || null,
            relatedInvoiceNumber: formData.relatedInvoiceNumber,
            farmId: getFarmIdForModule(formData) || null,
            cycleId: getCycleIdForModule(formData) || null,
          }),
        () => {
          toast.success("Faktura została zaktualizowana");
          onSave?.();
        },
        undefined,
        "Wystąpił błąd podczas aktualizacji faktury",
      );
    } finally {
      setSaving(false);
    }
  };

  // Handle accept
  // Helper function to convert string | number to number
  const toNumber = (value: string | number): number => {
    if (typeof value === "number") return value;
    const parsed = parseFormattedNumber(String(value));
    return parsed ? Number(parsed) : 0;
  };

  const handleAcceptClick = async () => {
    if (!details) return;

    const formData = watch();
    const currentModule = formData.moduleType || ModuleType.None;
    const requiresFarmAndCycle = [
      ModuleType.Feeds,
      ModuleType.Gas,
      ModuleType.ProductionExpenses,
      ModuleType.Sales,
    ].includes(currentModule as ModuleType);

    const farmId = getFarmIdForModule(formData);
    const cycleId = getCycleIdForModule(formData);

    if (requiresFarmAndCycle && !farmId) {
      toast.warning(
        "Wybierz lokalizację (fermę) przed zaakceptowaniem faktury",
      );
      return;
    }
    if (requiresFarmAndCycle && currentModule !== ModuleType.Gas && !cycleId) {
      toast.warning("Wybierz cykl przed zaakceptowaniem faktury");
      return;
    }

    // First save all changes
    await handleSubmit(async (formData) => {
      await handleSave(formData);

      // Build AcceptInvoiceRequest based on module type
      setSaving(true);
      try {
        await handleApiResponse(
          () => {
            const acceptData: any = {
              moduleType: formData.moduleType,
            };

            // Build module-specific data based on module type
            if (formData.moduleType === ModuleType.Feeds) {
              acceptData.feedData = {
                invoiceId: details.id,
                farmId: formData.feedFarmId,
                cycleId: formData.feedCycleId,
                henhouseId: formData.feedHenhouseId || undefined,
                invoiceNumber: formData.invoiceNumber,
                bankAccountNumber: formData.feedBankAccountNumber || "",
                vendorName: formData.feedVendorName || formData.sellerName,
                itemName: formData.feedItemName,
                quantity: toNumber(formData.feedQuantity),
                unitPrice: toNumber(formData.feedUnitPrice),
                invoiceDate: formData.invoiceDate,
                dueDate: formData.dueDate || formData.invoiceDate,
                invoiceTotal: formData.grossAmount,
                subTotal: formData.netAmount,
                vatAmount: formData.vatAmount,
                comment: formData.comment || "",
              };
            } else if (formData.moduleType === ModuleType.Gas) {
              acceptData.gasData = {
                invoiceId: details.id,
                farmId: formData.gasFarmId,
                contractorId: formData.gasContractorId || undefined,
                contractorNip: formData.sellerNip,
                contractorName: formData.sellerName,
                invoiceNumber: formData.invoiceNumber,
                invoiceDate: formData.invoiceDate,
                invoiceTotal: formData.grossAmount,
                unitPrice: toNumber(formData.gasUnitPrice),
                quantity: toNumber(formData.gasQuantity),
                comment: formData.comment || "",
              };
            } else if (formData.moduleType === ModuleType.ProductionExpenses) {
              acceptData.expenseData = {
                invoiceId: details.id,
                farmId: formData.expenseFarmId,
                cycleId: formData.expenseCycleId,
                expenseContractorId: formData.expenseContractorId || undefined,
                expenseTypeId: formData.expenseTypeId || undefined,
                contractorNip: formData.sellerNip,
                contractorName: formData.sellerName,
                invoiceNumber: formData.invoiceNumber,
                invoiceTotal: formData.grossAmount,
                subTotal: formData.netAmount,
                vatAmount: formData.vatAmount,
                invoiceDate: formData.invoiceDate,
                comment: formData.comment || "",
              };
            } else if (formData.moduleType === ModuleType.Sales) {
              acceptData.saleData = {
                invoiceId: details.id,
                farmId: formData.saleFarmId,
                cycleId: formData.saleCycleId,
                slaughterhouseId: formData.saleSlaughterhouseId || undefined,
                slaughterhouseNip: formData.buyerNip,
                slaughterhouseName: formData.buyerName,
                invoiceNumber: formData.invoiceNumber,
                invoiceDate: formData.invoiceDate,
                dueDate: formData.dueDate || formData.invoiceDate,
                invoiceTotal: formData.grossAmount,
                subTotal: formData.netAmount,
                vatAmount: formData.vatAmount,
              };
            }

            return AccountingService.acceptInvoice(details.id, acceptData);
          },
          () => {
            toast.success("Faktura została zaakceptowana i dodana do modułu");
            onSave?.();
          },
          undefined,
          "Błąd podczas akceptowania faktury",
        );
      } finally {
        setSaving(false);
      }
    })();
  };

  const renderPreview = () => {
    if (!previewFile) return <Typography>Brak podglądu</Typography>;

    return (
      <FilePreview
        file={previewFile}
        maxHeight={isLg ? 800 : isMd ? 650 : 500}
        showPreviewButton={true}
      />
    );
  };

  if (!details && !loading) {
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
              const num = Number(value);
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
                const parsed = parseFormattedNumber(e.target.value);
                field.onChange(parsed ? Number(parsed) : "");
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
              const num = Number(value);
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
                const parsed = parseFormattedNumber(e.target.value);
                field.onChange(parsed ? Number(parsed) : "");
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
              const num = Number(value);
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
                const parsed = parseFormattedNumber(e.target.value);
                field.onChange(parsed ? Number(parsed) : "");
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

      <Grid size={{ xs: 12, sm: 4 }}>
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

      <Grid size={{ xs: 12, sm: 4 }}>
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

      <Grid size={{ xs: 12, sm: 4 }}>
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
        <FormControl fullWidth>
          <InputLabel id="invoice-status-label">Status faktury</InputLabel>
          <Select
            labelId="invoice-status-label"
            label="Status faktury"
            value={watch("status") || KSeFInvoiceStatus.New}
            {...register("status")}
            disabled
          >
            {Object.entries(KSeFInvoiceStatusLabels).map(([key, label]) => (
              <MenuItem key={key} value={key}>
                {label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Grid>

      <Grid size={{ xs: 12, sm: 4 }}>
        <FormControl fullWidth>
          <InputLabel id="payment-status-label">Status płatności</InputLabel>
          <Select
            labelId="payment-status-label"
            label="Status płatności"
            value={watch("paymentStatus") || KSeFPaymentStatus.Unpaid}
            {...register("paymentStatus")}
          >
            {Object.entries(KSeFPaymentStatusLabels).map(([key, label]) => (
              <MenuItem key={key} value={key}>
                {label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Grid>

      {isPaidPaymentStatus && (
        <Grid size={{ xs: 12, sm: 4 }}>
          <Box
            sx={{
              backgroundColor: "#fff9c4",
              padding: "2px 6px",
              borderRadius: 1,
            }}
          >
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
          </Box>
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
    <AppDialog open={open} onClose={onClose} fullWidth maxWidth="xl">
      <DialogTitle>
        Szczegóły faktury {details?.invoiceNumber || ""}
      </DialogTitle>

      {loading ? (
        <DialogContent>
          <Box
            display="flex"
            justifyContent="center"
            alignItems="center"
            py={4}
          >
            <CircularProgress />
          </Box>
        </DialogContent>
      ) : details ? (
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
              {/* Left column - Preview and Attachments */}
              <Grid
                size={{ md: 12, lg: 5, xl: 6 }}
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

                {/* Attachments section */}
                <Box sx={{ mt: 3 }}>
                  <Divider sx={{ mb: 2 }}>
                    <Typography variant="caption">
                      Załączniki ({attachments.length})
                    </Typography>
                  </Divider>

                  <Box sx={{ mb: 2 }}>
                    <input
                      type="file"
                      ref={fileInputRef}
                      style={{ display: "none" }}
                      onChange={handleAttachmentUpload}
                      accept=".pdf,.jpg,.jpeg,.png,.doc,.docx,.xls,.xlsx"
                    />
                    <Button
                      variant="outlined"
                      size="small"
                      startIcon={<UploadFileIcon />}
                      onClick={() => fileInputRef.current?.click()}
                      disabled={uploadingAttachment}
                    >
                      {uploadingAttachment ? "Dodawanie..." : "Dodaj załącznik"}
                    </Button>
                  </Box>

                  {attachmentsLoading ? (
                    <CircularProgress size={24} />
                  ) : attachments.length === 0 ? (
                    <Typography variant="body2" color="text.secondary">
                      Brak załączników
                    </Typography>
                  ) : (
                    <Box
                      sx={{
                        display: "flex",
                        flexDirection: "column",
                        gap: 1,
                      }}
                    >
                      {attachments.map((attachment) => (
                        <Box
                          key={attachment.id}
                          sx={{
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "space-between",
                            p: 1,
                            border: 1,
                            borderColor: "divider",
                            borderRadius: 1,
                          }}
                        >
                          <Box>
                            <Typography variant="body2" fontWeight={500}>
                              {attachment.fileName}
                            </Typography>
                            <Typography
                              variant="caption"
                              color="text.secondary"
                            >
                              {(attachment.fileSize / 1024).toFixed(1)} KB •{" "}
                              {dayjs(attachment.uploadedAt).format(
                                "YYYY-MM-DD HH:mm",
                              )}
                            </Typography>
                          </Box>
                          <Box>
                            <Button
                              size="small"
                              onClick={() =>
                                handleAttachmentDownload(attachment)
                              }
                            >
                              <DownloadIcon fontSize="small" />
                            </Button>
                            <Button
                              size="small"
                              color="error"
                              onClick={() =>
                                handleAttachmentDelete(attachment.id)
                              }
                            >
                              <DeleteIcon fontSize="small" />
                            </Button>
                          </Box>
                        </Box>
                      ))}
                    </Box>
                  )}
                </Box>
              </Grid>

              {/* Right column - Form */}
              <Grid
                size={{ md: 12, lg: 7, xl: 6 }}
                sx={{
                  height: { lg: "100%" },
                  overflowY: { lg: "auto" },
                  p: 2,
                }}
              >
                <Grid container spacing={3} alignItems={"top"}>
                  <Grid size={12}>
                    <Typography variant="h6">Edycja faktury</Typography>
                    <Typography variant="body2" color="text.secondary">
                      Sprawdź i popraw dane faktury
                    </Typography>
                  </Grid>

                  {/* Status buttons at the top */}
                  <Grid size={12}>
                    <Box sx={{ mb: 2 }}>
                      <Typography
                        variant="body2"
                        color="text.secondary"
                        sx={{ mb: 1 }}
                      >
                        Zmień status faktury:
                      </Typography>
                      <ButtonGroup variant="outlined" fullWidth>
                        <Button
                          color="success"
                          variant={
                            details.status === KSeFInvoiceStatus.Accepted
                              ? "contained"
                              : "outlined"
                          }
                          startIcon={<CheckIcon />}
                          onClick={handleAcceptClick}
                          disabled={
                            saving ||
                            details.status === KSeFInvoiceStatus.Accepted
                          }
                        >
                          Zaakceptuj
                        </Button>
                        <Button
                          color="warning"
                          variant="outlined"
                          startIcon={<PauseIcon />}
                          onClick={() => {
                            setHoldUserId(watch("assignedUserId"));
                            setShowHoldModal(true);
                          }}
                          disabled={saving}
                        >
                          Wstrzymaj
                        </Button>
                        <Button
                          color="error"
                          variant={
                            details.status === KSeFInvoiceStatus.Rejected
                              ? "contained"
                              : "outlined"
                          }
                          startIcon={<CloseIcon />}
                          onClick={() =>
                            handleStatusChange(KSeFInvoiceStatus.Rejected)
                          }
                          disabled={saving}
                        >
                          Odrzuć
                        </Button>
                      </ButtonGroup>
                    </Box>
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
                        {users.map((user) => (
                          <MenuItem key={user.id} value={user.id}>
                            {user.name}
                          </MenuItem>
                        ))}
                      </Select>
                    </FormControl>
                  </Grid>

                  {/* Seller section */}
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

                  {/* Buyer section */}
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

                  {invoiceDetailsSection}
                  {classificationSection}
                  {statusSection}
                  {commentSection}

                  {/* Feeds module fields */}
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
                              onChange={(_, newValue) =>
                                onChange(newValue || "")
                              }
                              onInputChange={(_, newInputValue) => {
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

                  {/* Gas module fields */}
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
                        <Controller
                          name="gasQuantity"
                          control={control}
                          rules={{
                            required: "Ilość jest wymagana",
                            validate: (value) => {
                              const num = Number(value);
                              return (
                                (!isNaN(num) && num > 0) ||
                                "Ilość musi być liczbą większą od 0"
                              );
                            },
                          }}
                          render={({ field }) => (
                            <TextField
                              label="Ilość [l]"
                              value={formatNumberWithSpaces(field.value || "")}
                              onChange={(e) => {
                                const parsed = parseFormattedNumber(
                                  e.target.value,
                                );
                                field.onChange(parsed ? Number(parsed) : "");
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
                              const num = Number(value);
                              return (
                                (!isNaN(num) && num > 0) ||
                                "Cena musi być liczbą większą od 0"
                              );
                            },
                          }}
                          render={({ field }) => (
                            <TextField
                              label="Cena jednostkowa [zł/l]"
                              value={formatNumberWithSpaces(field.value || "")}
                              onChange={(e) => {
                                const parsed = parseFormattedNumber(
                                  e.target.value,
                                );
                                field.onChange(parsed ? Number(parsed) : "");
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
                      <Grid size={{ xs: 12, sm: 6 }}>
                        <Controller
                          name="gasContractorId"
                          control={control}
                          render={({ field: { onChange, value } }) => (
                            <Autocomplete
                              options={gasContractors}
                              getOptionLabel={(option) =>
                                typeof option === "string"
                                  ? option
                                  : `${option.name} (${option.nip})`
                              }
                              value={
                                gasContractors.find((c) => c.id === value) ||
                                null
                              }
                              onChange={(_, newValue) =>
                                onChange(newValue?.id || "")
                              }
                              renderInput={(params) => (
                                <TextField
                                  {...params}
                                  label="Kontrahent (Dostawca gazu)"
                                  error={!!errors.gasContractorId}
                                  helperText={errors.gasContractorId?.message}
                                />
                              )}
                            />
                          )}
                        />
                      </Grid>
                    </>
                  )}

                  {/* Production Expenses module fields */}
                  {watchedModuleType === ModuleType.ProductionExpenses && (
                    <>
                      <Grid size={12}>
                        <Divider>
                          <Typography variant="caption">
                            Dane kosztów produkcyjnych
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
                              return !!value || "Kontrahent jest wymagany";
                            },
                          }}
                          render={({ field: { onChange, value } }) => (
                            <Autocomplete
                              options={expenseContractors}
                              getOptionLabel={(option) => option.name || ""}
                              value={
                                expenseContractors.find(
                                  (c) => c.id === value,
                                ) || null
                              }
                              onChange={(_, newValue) => {
                                onChange(newValue?.id || "");
                                // Clear expense type if contractor changes
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
                                    errors.expenseContractorId?.message
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
                                      (c) =>
                                        c.id === watchedExpenseContractorId,
                                    );

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
                    </>
                  )}

                  {/* Sales module fields */}
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
                          value={watch("saleFarmId") || ""}
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
                          disabled={!watch("saleFarmId")}
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
                        <Controller
                          name="saleSlaughterhouseId"
                          control={control}
                          rules={{
                            validate: (value) => {
                              if (watchedModuleType !== ModuleType.Sales)
                                return true;
                              return !!value || "Ubojnia jest wymagana";
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
                              onChange={(_, newValue) =>
                                onChange(newValue?.id || "")
                              }
                              renderInput={(params) => (
                                <TextField
                                  {...params}
                                  label="Ubojnia (Nabywca)"
                                  required
                                  error={!!errors.saleSlaughterhouseId}
                                  helperText={
                                    errors.saleSlaughterhouseId?.message
                                  }
                                />
                              )}
                            />
                          )}
                        />
                      </Grid>
                    </>
                  )}
                </Grid>
              </Grid>
            </Grid>
          </DialogContent>

          <DialogActions
            sx={{
              justifyContent: sequentialMode ? "space-between" : "flex-end",
            }}
          >
            {sequentialMode && (
              <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                <Button
                  onClick={onPrevious}
                  disabled={currentIndex === 0}
                  variant="outlined"
                  size="small"
                >
                  Poprzednia
                </Button>
                <Typography variant="body2" sx={{ mx: 1 }}>
                  {currentIndex + 1} / {totalCount}
                </Typography>
                <Button
                  onClick={onNext}
                  disabled={currentIndex >= totalCount - 1}
                  variant="outlined"
                  size="small"
                >
                  Następna
                </Button>
              </Box>
            )}
            <Box sx={{ display: "flex", gap: 1 }}>
              {sequentialMode && (
                <Button
                  onClick={onExitSequential}
                  color="warning"
                  variant="outlined"
                >
                  Zakończ przeglądanie
                </Button>
              )}
              <Button onClick={onClose} disabled={saving}>
                Zamknij
              </Button>
              <LoadingButton
                type="submit"
                variant="contained"
                color="primary"
                startIcon={<MdSave />}
                disabled={saving}
                loading={saving}
              >
                Zapisz zmiany
              </LoadingButton>
            </Box>
          </DialogActions>
        </form>
      ) : (
        <DialogContent>
          <Typography
            color="text.secondary"
            sx={{ py: 4, textAlign: "center" }}
          >
            Brak danych do wyświetlenia
          </Typography>
        </DialogContent>
      )}

      {/* Hold Invoice Modal */}
      <AppDialog
        open={showHoldModal}
        onClose={() => setShowHoldModal(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Wstrzymaj fakturę</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Przypisz fakturę do innego pracownika. Faktura pozostanie jako
            "Nowa" do weryfikacji.
          </Typography>
          <FormControl fullWidth size="small">
            <InputLabel>Przypisz do pracownika</InputLabel>
            <Select
              value={holdUserId}
              label="Przypisz do pracownika"
              onChange={(e) => setHoldUserId(e.target.value)}
            >
              {users
                .filter((u) => u.id !== details?.assignedUserId)
                .map((user) => (
                  <MenuItem key={user.id} value={user.id}>
                    {user.name || user.login}
                  </MenuItem>
                ))}
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setShowHoldModal(false)}>Anuluj</Button>
          <Button
            variant="contained"
            color="warning"
            onClick={handleHoldInvoice}
            disabled={holdSaving || !holdUserId}
          >
            {holdSaving ? (
              <CircularProgress size={24} />
            ) : (
              "Wstrzymaj i przypisz"
            )}
          </Button>
        </DialogActions>
      </AppDialog>

      <ConfirmDialog
        open={Boolean(attachmentToDelete)}
        onClose={() => setAttachmentToDelete(null)}
        onConfirm={confirmAttachmentDelete}
        title="Usuń załącznik"
        content={
          attachmentToDelete
            ? `Czy na pewno chcesz usunąć załącznik "${attachmentToDelete.fileName}"?`
            : "Czy na pewno chcesz usunąć ten załącznik?"
        }
        confirmText="Usuń"
        confirmColor="error"
      />
    </AppDialog>
  );
};

export default NonKSeFInvoiceDetailsModal;
