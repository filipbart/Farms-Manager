import React, {
  useEffect,
  useState,
  useMemo,
  useCallback,
  useRef,
} from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  Typography,
  Grid,
  Chip,
  Divider,
  CircularProgress,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  ButtonGroup,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import CheckIcon from "@mui/icons-material/Check";
import PauseIcon from "@mui/icons-material/Pause";
import CloseIcon from "@mui/icons-material/Close";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import { toast } from "react-toastify";
import AppDialog from "../../common/app-dialog";
import { AccountingService } from "../../../services/accounting-service";
import { FarmsService } from "../../../services/farms-service";
import { UsersService } from "../../../services/users-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type {
  KSeFInvoiceDetails,
  KSeFInvoiceListModel,
  KSeFPartyData,
  LinkableInvoice,
} from "../../../models/accounting/ksef-invoice";
import {
  KSeFInvoiceStatusLabels,
  KSeFPaymentStatusLabels,
  KSeFInvoicePaymentTypeLabels,
  InvoiceSourceLabels,
  ModuleTypeLabels,
  KSeFInvoiceTypeLabels,
  VatDeductionTypeLabels,
  InvoiceRelationTypeLabels,
  KSeFInvoiceStatus,
  KSeFPaymentStatus,
  ModuleType,
  VatDeductionType,
  InvoiceRelationType,
} from "../../../models/accounting/ksef-invoice";
import type FarmRowModel from "../../../models/farms/farm-row-model";
import type CycleDto from "../../../models/farms/latest-cycle";
import type { UserListModel } from "../../../models/users/users";
import dayjs from "dayjs";
import { parseKSeFInvoiceXml } from "../../../utils/ksef-xml-parser";
import ModuleEntityForm, {
  type ModuleEntityFormRef,
} from "./module-entity-form";

// Funkcja konwertująca liczbę na słowa (po polsku)
const numberToWords = (num: number): string => {
  if (num === 0) return "zero";

  const ones = [
    "",
    "jeden",
    "dwa",
    "trzy",
    "cztery",
    "pięć",
    "sześć",
    "siedem",
    "osiem",
    "dziewięć",
  ];
  const teens = [
    "dziesięć",
    "jedenaście",
    "dwanaście",
    "trzynaście",
    "czternaście",
    "piętnaście",
    "szesnaście",
    "siedemnaście",
    "osiemnaście",
    "dziewiętnaście",
  ];
  const tens = [
    "",
    "",
    "dwadzieścia",
    "trzydzieści",
    "czterdzieści",
    "pięćdziesiąt",
    "sześćdziesiąt",
    "siedemdziesiąt",
    "osiemdziesiąt",
    "dziewięćdziesiąt",
  ];
  const hundreds = [
    "",
    "sto",
    "dwieście",
    "trzysta",
    "czterysta",
    "pięćset",
    "sześćset",
    "siedemset",
    "osiemset",
    "dziewięćset",
  ];

  const convertGroup = (n: number): string => {
    if (n === 0) return "";
    if (n < 10) return ones[n];
    if (n < 20) return teens[n - 10];
    if (n < 100)
      return tens[Math.floor(n / 10)] + (n % 10 ? " " + ones[n % 10] : "");
    return (
      hundreds[Math.floor(n / 100)] +
      (n % 100 ? " " + convertGroup(n % 100) : "")
    );
  };

  const intPart = Math.floor(num);
  const decPart = Math.round((num - intPart) * 100);

  let result = "";

  if (intPart >= 1000000) {
    const millions = Math.floor(intPart / 1000000);
    if (millions === 1) result += "milion ";
    else if (millions >= 2 && millions <= 4)
      result += convertGroup(millions) + " miliony ";
    else result += convertGroup(millions) + " milionów ";
  }

  if (intPart >= 1000) {
    const thousands = Math.floor((intPart % 1000000) / 1000);
    if (thousands > 0) {
      if (thousands === 1) result += "tysiąc ";
      else if (thousands >= 2 && thousands <= 4)
        result += convertGroup(thousands) + " tysiące ";
      else if (thousands >= 5 && thousands <= 21)
        result += convertGroup(thousands) + " tysięcy ";
      else {
        const lastDigit = thousands % 10;
        if (lastDigit >= 2 && lastDigit <= 4)
          result += convertGroup(thousands) + " tysiące ";
        else result += convertGroup(thousands) + " tysięcy ";
      }
    }
  }

  const remainder = intPart % 1000;
  if (remainder > 0) result += convertGroup(remainder);

  result = result.trim() + " PLN";

  if (decPart > 0) {
    result += " " + decPart.toString().padStart(2, "0") + "/100";
  } else {
    result += " 00/100";
  }

  return result;
};

// Funkcja obliczająca kwotę zapłaconą na podstawie statusu płatności
const calculatePaidAmount = (
  paymentStatus: KSeFPaymentStatus,
  isPaidFromXml: boolean | undefined,
  grossAmount: number
): number => {
  if (
    paymentStatus === KSeFPaymentStatus.PaidCash ||
    paymentStatus === KSeFPaymentStatus.PaidTransfer ||
    isPaidFromXml === true
  ) {
    return grossAmount;
  }
  if (paymentStatus === KSeFPaymentStatus.PartiallyPaid) {
    return grossAmount * 0.5; // Zakładamy 50% dla częściowej płatności
  }
  return 0;
};

interface InvoiceDetailsModalProps {
  open: boolean;
  onClose: () => void;
  onSave?: () => void;
  invoice: KSeFInvoiceListModel | null;
}

const DetailRow: React.FC<{ label: string; value: React.ReactNode }> = ({
  label,
  value,
}) => (
  <Grid container spacing={1} sx={{ mb: 1 }}>
    <Grid size={{ xs: 12, sm: 5 }}>
      <Typography variant="body2" color="text.secondary" fontWeight={500}>
        {label}:
      </Typography>
    </Grid>
    <Grid size={{ xs: 12, sm: 7 }}>
      <Typography variant="body2">{value || "—"}</Typography>
    </Grid>
  </Grid>
);

const getStatusColor = (status: KSeFInvoiceStatus) => {
  switch (status) {
    case KSeFInvoiceStatus.Accepted:
      return "success";
    case KSeFInvoiceStatus.Rejected:
      return "error";
    case KSeFInvoiceStatus.SentToOffice:
      return "info";
    case KSeFInvoiceStatus.New:
    default:
      return "warning";
  }
};

const getPaymentStatusColor = (status: KSeFPaymentStatus) => {
  switch (status) {
    case KSeFPaymentStatus.PaidCash:
    case KSeFPaymentStatus.PaidTransfer:
      return "success";
    case KSeFPaymentStatus.Suspended:
      return "error";
    case KSeFPaymentStatus.PartiallyPaid:
      return "warning";
    case KSeFPaymentStatus.Unpaid:
    default:
      return "default";
  }
};

const formatAddress = (party: KSeFPartyData | undefined): string => {
  if (!party?.address) return "—";
  const parts = [
    party.address.addressLine1,
    party.address.addressLine2,
    party.address.countryCode,
  ].filter(Boolean);
  return parts.length > 0 ? parts.join(", ") : "—";
};

const formatAddressLines = (party: KSeFPartyData | undefined): string[] => {
  if (!party?.address) return [];
  const lines: string[] = [];
  if (party.address.addressLine1) lines.push(party.address.addressLine1);
  if (party.address.addressLine2) lines.push(party.address.addressLine2);
  return lines;
};

const CopyableText: React.FC<{
  value: string | undefined | null;
  children?: React.ReactNode;
  hoverColor?: string;
}> = ({ value, children, hoverColor = "primary.main" }) => {
  const handleCopy = () => {
    if (value) {
      navigator.clipboard.writeText(value);
      toast.success("Skopiowano do schowka", { autoClose: 1500 });
    }
  };

  if (!value) {
    return <>{children || "—"}</>;
  }

  return (
    <Box
      component="span"
      onClick={handleCopy}
      sx={{
        cursor: "pointer",
        display: "inline-flex",
        alignItems: "center",
        gap: 0.5,
        "&:hover": {
          color: hoverColor,
          "& .copy-icon": {
            opacity: 1,
          },
        },
      }}
      title="Kliknij, aby skopiować"
    >
      {children || value}
      <ContentCopyIcon
        className="copy-icon"
        sx={{ fontSize: 14, opacity: 0, transition: "opacity 0.2s" }}
      />
    </Box>
  );
};

const InvoicePartyBox: React.FC<{
  label: string;
  party: KSeFPartyData | undefined;
  fallbackName?: string;
  fallbackNip?: string;
}> = ({ label, party, fallbackName, fallbackNip }) => (
  <Box sx={{ flex: 1 }}>
    <Typography
      variant="caption"
      color="text.secondary"
      sx={{
        textTransform: "uppercase",
        fontSize: "0.65rem",
        letterSpacing: 0.5,
      }}
    >
      {label}:
    </Typography>
    <Typography variant="body2" fontWeight={600} sx={{ mt: 0.5 }}>
      <CopyableText value={party?.name || fallbackName}>
        {party?.name || fallbackName || "—"}
      </CopyableText>
    </Typography>
    {formatAddressLines(party).map((line, idx) => (
      <Typography key={idx} variant="body2" color="text.secondary">
        <CopyableText value={line}>{line}</CopyableText>
      </Typography>
    ))}
    <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
      <strong>NIP:</strong>{" "}
      <CopyableText value={party?.nip || fallbackNip}>
        {party?.nip || fallbackNip || "—"}
      </CopyableText>
    </Typography>
    {party?.contact?.phone && (
      <Typography variant="body2" color="text.secondary">
        <strong>NUMER TELEFONU:</strong>{" "}
        <CopyableText value={party.contact.phone}>
          {party.contact.phone}
        </CopyableText>
      </Typography>
    )}
  </Box>
);

const InvoiceInfoRow: React.FC<{
  label: string;
  value: React.ReactNode;
  copyValue?: string;
}> = ({ label, value, copyValue }) => (
  <Box sx={{ display: "flex", mb: 0.5 }}>
    <Typography
      variant="caption"
      color="text.secondary"
      sx={{ minWidth: 140, textTransform: "uppercase", fontSize: "0.65rem" }}
    >
      {label}:
    </Typography>
    <Typography variant="body2">
      {copyValue ? (
        <CopyableText value={copyValue}>{value || "—"}</CopyableText>
      ) : (
        value || "—"
      )}
    </Typography>
  </Box>
);

interface EditFormState {
  moduleType: ModuleType;
  vatDeductionType: VatDeductionType;
  paymentStatus: KSeFPaymentStatus;
  farmId: string;
  cycleId: string;
  assignedUserId: string;
  comment: string;
  relatedInvoiceNumber: string;
}

const InvoiceDetailsModal: React.FC<InvoiceDetailsModalProps> = ({
  open,
  onClose,
  onSave,
  invoice,
}) => {
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [details, setDetails] = useState<KSeFInvoiceDetails | null>(null);

  // Edit form state
  const [editForm, setEditForm] = useState<EditFormState>({
    moduleType: ModuleType.None,
    vatDeductionType: VatDeductionType.Full,
    paymentStatus: KSeFPaymentStatus.Unpaid,
    farmId: "",
    cycleId: "",
    assignedUserId: "",
    comment: "",
    relatedInvoiceNumber: "",
  });

  // Data for dropdowns
  const [farms, setFarms] = useState<FarmRowModel[]>([]);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [users, setUsers] = useState<UserListModel[]>([]);

  // Linking state
  const [linkableInvoices, setLinkableInvoices] = useState<LinkableInvoice[]>(
    []
  );
  const [linkingSearch, setLinkingSearch] = useState("");
  const [selectedLinkIds, setSelectedLinkIds] = useState<string[]>([]);
  const [relationType, setRelationType] = useState<InvoiceRelationType>(
    InvoiceRelationType.CorrectionToOriginal
  );
  const [linkingLoading, setLinkingLoading] = useState(false);

  // Module entity form state
  const [showModuleForm, setShowModuleForm] = useState(false);
  const [pendingModuleType, setPendingModuleType] = useState<string | null>(
    null
  );
  const moduleFormRef = useRef<ModuleEntityFormRef>(null);

  // Hold modal state
  const [showHoldModal, setShowHoldModal] = useState(false);
  const [holdUserId, setHoldUserId] = useState<string>("");
  const [holdSaving, setHoldSaving] = useState(false);

  const parsedXml = useMemo(() => {
    if (!details?.invoiceXml) return null;
    return parseKSeFInvoiceXml(details.invoiceXml);
  }, [details?.invoiceXml]);

  // Initialize form when details are loaded
  useEffect(() => {
    if (details) {
      const currentModuleType = details.moduleType || ModuleType.None;

      // Auto-select active cycle if farm is set but cycle is not
      let cycleIdToUse = details.cycleId || "";
      if (details.farmId && !details.cycleId && farms.length > 0) {
        const farm = farms.find((f) => f.id === details.farmId);
        if (farm?.activeCycle?.id) {
          cycleIdToUse = farm.activeCycle.id;
        }
      }

      setEditForm({
        moduleType: currentModuleType,
        vatDeductionType: details.vatDeductionType || VatDeductionType.Full,
        paymentStatus: details.paymentStatus || KSeFPaymentStatus.Unpaid,
        farmId: details.farmId || "",
        cycleId: cycleIdToUse,
        assignedUserId: details.assignedUserId || "",
        comment: details.comment || "",
        relatedInvoiceNumber: details.relatedInvoiceNumber || "",
      });

      // Auto-show module form if module type requires entity creation
      const requiresEntity = [
        ModuleType.Feeds,
        ModuleType.Gas,
        ModuleType.ProductionExpenses,
        ModuleType.Sales,
      ].includes(currentModuleType as ModuleType);

      if (requiresEntity) {
        setPendingModuleType(currentModuleType);
        setShowModuleForm(true);
      } else {
        setPendingModuleType(null);
        setShowModuleForm(false);
      }
    }
  }, [details, farms]);

  // Fetch farms and users
  useEffect(() => {
    const fetchDropdownData = async () => {
      try {
        const [farmsRes, usersRes] = await Promise.all([
          FarmsService.getFarmsAsync(),
          UsersService.getUsers({ page: 0, pageSize: 100 }),
        ]);
        if (farmsRes.success && farmsRes.responseData) {
          setFarms(farmsRes.responseData.items || []);
        }
        if (usersRes.success && usersRes.responseData) {
          setUsers(usersRes.responseData.items || []);
        }
      } catch {
        // Ignore errors for dropdown data
      }
    };
    if (open) {
      fetchDropdownData();
    }
  }, [open]);

  // Fetch cycles when farm changes
  useEffect(() => {
    const fetchCycles = async () => {
      if (!editForm.farmId) {
        setCycles([]);
        return;
      }
      try {
        const res = await FarmsService.getFarmCycles(editForm.farmId);
        if (res.success && res.responseData) {
          setCycles(res.responseData);
        }
      } catch {
        setCycles([]);
      }
    };
    fetchCycles();
  }, [editForm.farmId]);

  const handleFormChange = useCallback(
    (field: keyof EditFormState, value: string) => {
      setEditForm((prev) => ({ ...prev, [field]: value }));
      // Auto-select active cycle when farm changes
      if (field === "farmId") {
        const selectedFarm = farms.find((f) => f.id === value);
        const activeCycleId = selectedFarm?.activeCycle?.id || "";
        setEditForm((prev) => ({ ...prev, cycleId: activeCycleId }));
      }
    },
    [farms]
  );

  const handleStatusChange = useCallback(
    async (newStatus: KSeFInvoiceStatus) => {
      if (!details) return;
      setSaving(true);
      try {
        await handleApiResponse(
          () =>
            AccountingService.updateInvoice(details.id, { status: newStatus }),
          () => {
            toast.success("Status faktury został zmieniony");
            setDetails((prev) =>
              prev ? { ...prev, status: newStatus } : null
            );
            onSave?.();
          },
          undefined,
          "Błąd podczas zmiany statusu"
        );
      } finally {
        setSaving(false);
      }
    },
    [details, onSave]
  );

  // Hold invoice handler - saves all form changes without changing status
  const handleHoldInvoice = useCallback(async () => {
    if (!details || !holdUserId) {
      toast.warning("Wybierz pracownika");
      return;
    }
    setHoldSaving(true);
    try {
      // Save all form changes (module, farm, cycle, assigned user) without changing status
      await handleApiResponse(
        () =>
          AccountingService.updateInvoice(details.id, {
            moduleType: editForm.moduleType,
            farmId: editForm.farmId || null,
            cycleId: editForm.cycleId || null,
            assignedUserId: holdUserId,
            vatDeductionType: editForm.vatDeductionType,
            paymentStatus: editForm.paymentStatus,
            comment: editForm.comment,
            relatedInvoiceNumber: editForm.relatedInvoiceNumber,
          }),
        () => {
          toast.success(
            "Zmiany zostały zapisane i faktura przypisana do pracownika"
          );
          setShowHoldModal(false);
          setHoldUserId("");
          onSave?.();
        },
        undefined,
        "Błąd podczas zapisywania zmian"
      );
    } finally {
      setHoldSaving(false);
    }
  }, [details, holdUserId, editForm, onSave]);

  // Check if module requires entity creation
  const moduleRequiresEntity = useCallback((moduleType: string) => {
    return [
      ModuleType.Feeds,
      ModuleType.Gas,
      ModuleType.ProductionExpenses,
      ModuleType.Sales,
    ].includes(moduleType as ModuleType);
  }, []);

  // Handle accept button click - validates farm/cycle and submits form directly
  const handleAcceptClick = useCallback(async () => {
    if (!details) return;

    // Validate farm and cycle are selected
    if (!editForm.farmId) {
      toast.warning(
        "Wybierz lokalizację (fermę) przed zaakceptowaniem faktury"
      );
      return;
    }
    if (!editForm.cycleId) {
      toast.warning("Wybierz cykl przed zaakceptowaniem faktury");
      return;
    }

    const currentModule = editForm.moduleType || ModuleType.None;
    if (moduleRequiresEntity(currentModule)) {
      // Submit the module form directly (creates entry and changes status)
      if (moduleFormRef.current) {
        await moduleFormRef.current.submit();
      }
    } else {
      // For modules that don't require entity (Farmstead, Other, None), accept directly
      handleStatusChange(KSeFInvoiceStatus.Accepted);
    }
  }, [
    details,
    editForm.farmId,
    editForm.cycleId,
    editForm.moduleType,
    moduleRequiresEntity,
    handleStatusChange,
  ]);

  // Linking handlers
  const fetchLinkableInvoices = useCallback(
    async (search?: string) => {
      if (!details) return;
      setLinkingLoading(true);
      try {
        const res = await AccountingService.getLinkableInvoices(
          details.id,
          search
        );
        if (res.success && res.responseData) {
          setLinkableInvoices(res.responseData);
        }
      } catch {
        // Ignore errors
      } finally {
        setLinkingLoading(false);
      }
    },
    [details]
  );

  const handleLinkInvoices = useCallback(async () => {
    if (!details || selectedLinkIds.length === 0) return;
    setSaving(true);
    try {
      await handleApiResponse(
        () =>
          AccountingService.linkInvoices({
            sourceInvoiceId: details.id,
            targetInvoiceIds: selectedLinkIds,
            relationType: relationType,
          }),
        () => {
          toast.success("Faktury zostały powiązane");
          setSelectedLinkIds([]);
          setDetails((prev) =>
            prev ? { ...prev, status: KSeFInvoiceStatus.New } : null
          );
          onSave?.();
        },
        undefined,
        "Błąd podczas powiązywania faktur"
      );
    } finally {
      setSaving(false);
    }
  }, [details, selectedLinkIds, relationType, onSave]);

  const handleAcceptNoLinking = useCallback(async () => {
    if (!details) return;
    setSaving(true);
    try {
      await handleApiResponse(
        () => AccountingService.acceptNoLinking(details.id),
        () => {
          toast.success("Zaakceptowano brak powiązania");
          setDetails((prev) =>
            prev ? { ...prev, status: KSeFInvoiceStatus.New } : null
          );
          onSave?.();
        },
        undefined,
        "Błąd podczas akceptacji braku powiązania"
      );
    } finally {
      setSaving(false);
    }
  }, [details, onSave]);

  const handlePostponeLinking = useCallback(async () => {
    if (!details) return;
    setSaving(true);
    try {
      await handleApiResponse(
        () => AccountingService.postponeLinkingReminder(details.id, 3),
        () => {
          toast.success("Przypomnienie odłożone o 3 dni");
        },
        undefined,
        "Błąd podczas odkładania przypomnienia"
      );
    } finally {
      setSaving(false);
    }
  }, [details]);

  // Fetch linkable invoices when status requires linking
  useEffect(() => {
    if (details?.status === KSeFInvoiceStatus.RequiresLinking) {
      fetchLinkableInvoices();
    }
  }, [details?.status, fetchLinkableInvoices]);

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
          "Błąd podczas pobierania szczegółów faktury"
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

  const formatCurrency = (value: number | undefined) => {
    if (value === undefined || value === null) return "—";
    return (
      value.toLocaleString("pl-PL", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }) + " zł"
    );
  };

  return (
    <AppDialog open={open} onClose={onClose} maxWidth="xl" fullWidth>
      <DialogTitle>
        Szczegóły faktury {invoice?.invoiceNumber || ""}
      </DialogTitle>
      <DialogContent>
        {loading ? (
          <Box
            display="flex"
            justifyContent="center"
            alignItems="center"
            py={4}
          >
            <CircularProgress />
          </Box>
        ) : details ? (
          <Grid container spacing={3} sx={{ mt: 1 }}>
            {/* Left side - Invoice visualization (styled like classic invoice) */}
            <Grid size={{ xs: 12, md: 8 }}>
              <Paper
                variant="outlined"
                sx={{
                  p: 3,
                  mr: { md: 2 },
                  borderColor: "divider",
                  backgroundColor: "background.paper",
                }}
              >
                {/* Invoice Header */}
                <Box sx={{ mb: 3 }}>
                  <Typography
                    variant="h4"
                    sx={{
                      color: "primary.main",
                      fontWeight: 700,
                      fontStyle: "italic",
                      mb: 1,
                    }}
                  >
                    Faktura
                  </Typography>
                  <Box
                    sx={{
                      display: "inline-block",
                      bgcolor: "grey.200",
                      px: 1.5,
                      py: 0.5,
                      borderRadius: 1,
                    }}
                  >
                    <Typography variant="body2" fontWeight={500}>
                      Nr{" "}
                      <CopyableText value={details.invoiceNumber}>
                        {details.invoiceNumber}
                      </CopyableText>
                    </Typography>
                  </Box>
                  {details.kSeFNumber && (
                    <Typography
                      variant="caption"
                      color="text.secondary"
                      sx={{ ml: 2 }}
                    >
                      KSeF:{" "}
                      <CopyableText value={details.kSeFNumber}>
                        {details.kSeFNumber}
                      </CopyableText>
                    </Typography>
                  )}
                </Box>

                {/* Seller and Buyer Section - side by side */}
                <Box
                  sx={{
                    display: "flex",
                    gap: 4,
                    mb: 3,
                    pb: 2,
                    borderBottom: 1,
                    borderColor: "primary.main",
                  }}
                >
                  <InvoicePartyBox
                    label="Sprzedawca"
                    party={parsedXml?.seller}
                    fallbackName={details.sellerName}
                    fallbackNip={details.sellerNip}
                  />
                  <InvoicePartyBox
                    label="Nabywca"
                    party={parsedXml?.buyer}
                    fallbackName={details.buyerName}
                    fallbackNip={details.buyerNip}
                  />
                </Box>

                {/* Dates and Payment Info Section - side by side */}
                <Box
                  sx={{
                    display: "flex",
                    gap: 4,
                    mb: 3,
                    pb: 2,
                    borderBottom: 1,
                    borderColor: "primary.main",
                  }}
                >
                  {/* Dates */}
                  <Box sx={{ flex: 1 }}>
                    <InvoiceInfoRow
                      label="Data wystawienia"
                      value={
                        details.invoiceDate
                          ? dayjs(details.invoiceDate).format("YYYY-MM-DD")
                          : "—"
                      }
                      copyValue={
                        details.invoiceDate
                          ? dayjs(details.invoiceDate).format("YYYY-MM-DD")
                          : undefined
                      }
                    />
                    {parsedXml?.invoiceData?.saleDate && (
                      <InvoiceInfoRow
                        label="Data sprzedaży"
                        value={dayjs(parsedXml.invoiceData.saleDate).format(
                          "YYYY-MM-DD"
                        )}
                        copyValue={dayjs(parsedXml.invoiceData.saleDate).format(
                          "YYYY-MM-DD"
                        )}
                      />
                    )}
                    {parsedXml?.payment?.dueDate && (
                      <InvoiceInfoRow
                        label="Termin płatności"
                        value={dayjs(parsedXml.payment.dueDate).format(
                          "YYYY-MM-DD"
                        )}
                        copyValue={dayjs(parsedXml.payment.dueDate).format(
                          "YYYY-MM-DD"
                        )}
                      />
                    )}
                  </Box>
                  {/* Payment Info */}
                  <Box sx={{ flex: 1 }}>
                    <InvoiceInfoRow
                      label="Sposób płatności"
                      value={
                        parsedXml?.payment?.paymentMethod ||
                        KSeFInvoicePaymentTypeLabels[details.paymentType] ||
                        details.paymentType
                      }
                    />
                    {parsedXml?.payment?.bankAccounts?.[0] && (
                      <>
                        {parsedXml.payment.bankAccounts[0].bankName && (
                          <InvoiceInfoRow
                            label="Bank"
                            value={parsedXml.payment.bankAccounts[0].bankName}
                            copyValue={
                              parsedXml.payment.bankAccounts[0].bankName
                            }
                          />
                        )}
                        <InvoiceInfoRow
                          label="Numer konta"
                          value={
                            parsedXml.payment.bankAccounts[0].accountNumber
                          }
                          copyValue={
                            parsedXml.payment.bankAccounts[0].accountNumber
                          }
                        />
                      </>
                    )}
                  </Box>
                </Box>

                {/* Line Items Table */}
                <TableContainer sx={{ mb: 2 }}>
                  <Table size="small">
                    <TableHead>
                      <TableRow
                        sx={{
                          bgcolor: "primary.main",
                          "& th": { color: "white", fontWeight: 600, py: 1 },
                        }}
                      >
                        <TableCell>Lp.</TableCell>
                        <TableCell>Nazwa</TableCell>
                        <TableCell align="center">Ilość</TableCell>
                        <TableCell align="center">Jm</TableCell>
                        <TableCell align="right">Cena netto</TableCell>
                        <TableCell align="right">Wartość netto</TableCell>
                        <TableCell align="center">Stawka VAT</TableCell>
                        <TableCell align="right">Kwota VAT</TableCell>
                        <TableCell align="right">Wartość brutto</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {parsedXml?.lineItems &&
                      parsedXml.lineItems.length > 0 ? (
                        parsedXml.lineItems.map((item) => (
                          <TableRow key={item.lineNumber}>
                            <TableCell>{item.lineNumber}</TableCell>
                            <TableCell>
                              <Typography
                                variant="body2"
                                sx={{ maxWidth: 180 }}
                              >
                                {item.name || "—"}
                              </Typography>
                            </TableCell>
                            <TableCell align="center">
                              {item.quantity?.toLocaleString("pl-PL") || "—"}
                            </TableCell>
                            <TableCell align="center">
                              {item.unit || "—"}
                            </TableCell>
                            <TableCell align="right">
                              {formatCurrency(item.unitPriceNet)}
                            </TableCell>
                            <TableCell align="right">
                              {formatCurrency(item.netAmount)}
                            </TableCell>
                            <TableCell align="center">
                              {item.vatRate !== undefined
                                ? `${item.vatRate}%`
                                : "zw."}
                            </TableCell>
                            <TableCell align="right">
                              {(() => {
                                // Oblicz VAT: jeśli jest grossAmount, użyj różnicy, w przeciwnym razie oblicz z vatRate
                                if (
                                  item.grossAmount !== undefined &&
                                  item.netAmount !== undefined
                                ) {
                                  return formatCurrency(
                                    item.grossAmount - item.netAmount
                                  );
                                }
                                if (
                                  item.netAmount !== undefined &&
                                  item.vatRate !== undefined
                                ) {
                                  return formatCurrency(
                                    item.netAmount * (item.vatRate / 100)
                                  );
                                }
                                return formatCurrency(0);
                              })()}
                            </TableCell>
                            <TableCell align="right">
                              {(() => {
                                // Oblicz brutto: jeśli jest grossAmount użyj go, w przeciwnym razie oblicz z netto + VAT
                                if (item.grossAmount !== undefined) {
                                  return formatCurrency(item.grossAmount);
                                }
                                if (item.netAmount !== undefined) {
                                  const vatAmount =
                                    item.vatRate !== undefined
                                      ? item.netAmount * (item.vatRate / 100)
                                      : 0;
                                  return formatCurrency(
                                    item.netAmount + vatAmount
                                  );
                                }
                                return "—";
                              })()}
                            </TableCell>
                          </TableRow>
                        ))
                      ) : (
                        <TableRow>
                          <TableCell colSpan={9} align="center">
                            <Typography variant="body2" color="text.secondary">
                              Brak pozycji do wyświetlenia
                            </Typography>
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>

                {/* Summary Section */}
                <Box sx={{ display: "flex", gap: 2, mb: 3 }}>
                  {/* Total to pay box */}
                  <Box
                    sx={{
                      bgcolor: "primary.main",
                      color: "white",
                      px: 3,
                      py: 1.5,
                      borderRadius: 1,
                      display: "flex",
                      alignItems: "center",
                      gap: 2,
                    }}
                  >
                    <Typography variant="body2" fontWeight={500}>
                      RAZEM DO ZAPŁATY:
                    </Typography>
                    <Typography variant="h6" fontWeight={700}>
                      <CopyableText
                        value={details.grossAmount?.toFixed(2)}
                        hoverColor="primary.light"
                      >
                        {formatCurrency(details.grossAmount)}
                      </CopyableText>
                    </Typography>
                  </Box>

                  {/* VAT Summary Table */}
                  <Box sx={{ flex: 1 }}>
                    <Table size="small">
                      <TableHead>
                        <TableRow
                          sx={{ "& th": { py: 0.5, fontSize: "0.75rem" } }}
                        >
                          <TableCell></TableCell>
                          <TableCell align="right">Wartość netto</TableCell>
                          <TableCell align="center">Stawka</TableCell>
                          <TableCell align="right">Kwota VAT</TableCell>
                          <TableCell align="right">Wartość brutto</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        <TableRow
                          sx={{ "& td": { py: 0.5, fontSize: "0.8rem" } }}
                        >
                          <TableCell
                            sx={{ color: "primary.main", fontWeight: 600 }}
                          >
                            Razem:
                          </TableCell>
                          <TableCell
                            align="right"
                            sx={{ color: "primary.main" }}
                          >
                            <CopyableText value={details.netAmount?.toFixed(2)}>
                              {formatCurrency(details.netAmount)}
                            </CopyableText>
                          </TableCell>
                          <TableCell
                            align="center"
                            sx={{ color: "primary.main" }}
                          >
                            X
                          </TableCell>
                          <TableCell
                            align="right"
                            sx={{ color: "primary.main" }}
                          >
                            <CopyableText value={details.vatAmount?.toFixed(2)}>
                              {formatCurrency(details.vatAmount)}
                            </CopyableText>
                          </TableCell>
                          <TableCell
                            align="right"
                            sx={{ color: "primary.main" }}
                          >
                            <CopyableText
                              value={details.grossAmount?.toFixed(2)}
                            >
                              {formatCurrency(details.grossAmount)}
                            </CopyableText>
                          </TableCell>
                        </TableRow>
                        {parsedXml?.invoiceData?.vatBreakdown?.map(
                          (vat, idx) => (
                            <TableRow
                              key={idx}
                              sx={{ "& td": { py: 0.5, fontSize: "0.75rem" } }}
                            >
                              <TableCell>W tym:</TableCell>
                              <TableCell align="right">
                                {formatCurrency(vat.netAmount)}
                              </TableCell>
                              <TableCell align="center">
                                {vat.rate || "zw."}
                              </TableCell>
                              <TableCell align="right">
                                {vat.vatAmount !== undefined
                                  ? formatCurrency(vat.vatAmount)
                                  : formatCurrency(0)}
                              </TableCell>
                              <TableCell align="right">
                                {formatCurrency(
                                  (vat.netAmount || 0) + (vat.vatAmount || 0)
                                )}
                              </TableCell>
                            </TableRow>
                          )
                        )}
                      </TableBody>
                    </Table>
                  </Box>
                </Box>

                {/* Payment Summary */}
                <Box sx={{ mb: 2 }}>
                  {(() => {
                    const paidAmount = calculatePaidAmount(
                      details.paymentStatus,
                      parsedXml?.payment?.isPaid,
                      details.grossAmount || 0
                    );
                    const remainingAmount =
                      (details.grossAmount || 0) - paidAmount;
                    return (
                      <>
                        <Typography variant="body2">
                          <strong>Zapłacono:</strong>{" "}
                          {formatCurrency(paidAmount)}{" "}
                          <strong style={{ marginLeft: 16 }}>
                            Pozostało do zapłaty:
                          </strong>{" "}
                          {formatCurrency(remainingAmount)}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                          <strong>Słownie:</strong>{" "}
                          {numberToWords(details.grossAmount || 0)}
                        </Typography>
                      </>
                    );
                  })()}
                </Box>

                {/* Footer / Notes */}
                {(parsedXml?.footer || details.comment) && (
                  <Box
                    sx={{
                      mt: 2,
                      pt: 2,
                      borderTop: 1,
                      borderColor: "divider",
                    }}
                  >
                    <Typography variant="body2" color="text.secondary">
                      <strong>Uwagi:</strong>
                    </Typography>
                    {parsedXml?.footer && (
                      <Typography
                        variant="body2"
                        color="text.secondary"
                        sx={{ whiteSpace: "pre-wrap" }}
                      >
                        {parsedXml.footer}
                      </Typography>
                    )}
                    {details.comment && (
                      <Typography
                        variant="body2"
                        color="text.secondary"
                        sx={{ whiteSpace: "pre-wrap", mt: 1 }}
                      >
                        {details.comment}
                      </Typography>
                    )}
                  </Box>
                )}

                {/* Additional Info Accordion */}
                <Accordion sx={{ mt: 2 }}>
                  <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                    <Typography fontWeight={600}>
                      Dodatkowe informacje
                    </Typography>
                  </AccordionSummary>
                  <AccordionDetails>
                    <Box sx={{ display: "flex", flexWrap: "wrap", gap: 3 }}>
                      <Box sx={{ minWidth: 200 }}>
                        <DetailRow
                          label="Typ faktury"
                          value={
                            KSeFInvoiceTypeLabels[details.invoiceType] ||
                            details.invoiceType
                          }
                        />
                        <DetailRow
                          label="Źródło"
                          value={
                            <Chip
                              label={
                                InvoiceSourceLabels[details.source] ||
                                details.source
                              }
                              size="small"
                              color={
                                details.source === "KSeF"
                                  ? "primary"
                                  : "default"
                              }
                              variant="outlined"
                            />
                          }
                        />
                        <DetailRow
                          label="Status faktury"
                          value={
                            <Chip
                              label={
                                KSeFInvoiceStatusLabels[details.status] ||
                                details.status
                              }
                              size="small"
                              color={getStatusColor(details.status)}
                            />
                          }
                        />
                        <DetailRow
                          label="Status płatności"
                          value={
                            <Chip
                              label={
                                KSeFPaymentStatusLabels[
                                  details.paymentStatus
                                ] || details.paymentStatus
                              }
                              size="small"
                              color={getPaymentStatusColor(
                                details.paymentStatus
                              )}
                              variant="outlined"
                            />
                          }
                        />
                      </Box>
                      <Box sx={{ minWidth: 200 }}>
                        <DetailRow
                          label="Moduł"
                          value={
                            details.moduleType
                              ? ModuleTypeLabels[details.moduleType] ||
                                details.moduleType
                              : "—"
                          }
                        />
                        <DetailRow
                          label="Lokalizacja"
                          value={details.location}
                        />
                        <DetailRow
                          label="Przypisany użytkownik"
                          value={details.assignedUserName}
                        />
                        <DetailRow
                          label="Identyfikator cyklu"
                          value={
                            details.cycleIdentifier && details.cycleYear
                              ? `${details.cycleIdentifier}/${details.cycleYear}`
                              : "—"
                          }
                        />
                      </Box>
                    </Box>
                    {parsedXml?.thirdParty && (
                      <Box sx={{ mt: 2 }}>
                        <Typography
                          variant="body2"
                          fontWeight={600}
                          gutterBottom
                        >
                          Podmiot trzeci{" "}
                          {parsedXml.thirdParty.role
                            ? `(${parsedXml.thirdParty.role})`
                            : ""}
                        </Typography>
                        <DetailRow
                          label="Nazwa"
                          value={parsedXml.thirdParty.name}
                        />
                        <DetailRow
                          label="NIP"
                          value={parsedXml.thirdParty.nip}
                        />
                        <DetailRow
                          label="Adres"
                          value={formatAddress(parsedXml.thirdParty)}
                        />
                      </Box>
                    )}
                  </AccordionDetails>
                </Accordion>
              </Paper>
            </Grid>

            {/* Right side - Edit panel */}
            <Grid size={{ xs: 12, md: 4 }}>
              <Box sx={{ pl: { md: 2 } }}>
                <Typography variant="subtitle1" fontWeight={600} gutterBottom>
                  Edycja faktury
                </Typography>

                {/* Status buttons */}
                <Box sx={{ mb: 3 }}>
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
                        saving || details.status === KSeFInvoiceStatus.Accepted
                      }
                    >
                      Zaakceptuj
                    </Button>
                    <Button
                      color="warning"
                      variant="outlined"
                      startIcon={<PauseIcon />}
                      onClick={() => {
                        setHoldUserId(editForm.assignedUserId);
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

                {/* Linking section - shown when invoice requires linking */}
                {details.status === KSeFInvoiceStatus.RequiresLinking && (
                  <>
                    <Divider sx={{ my: 2 }} />
                    <Box
                      sx={{
                        mb: 2,
                        p: 2,
                        bgcolor: "warning.light",
                        borderRadius: 1,
                      }}
                    >
                      <Typography
                        variant="subtitle2"
                        fontWeight={600}
                        sx={{ mb: 1 }}
                      >
                        Ta faktura wymaga powiązania
                      </Typography>
                      <Typography variant="body2" sx={{ mb: 2 }}>
                        Wybierz faktury do powiązania lub zaakceptuj brak
                        powiązania.
                      </Typography>

                      <TextField
                        fullWidth
                        size="small"
                        placeholder="Szukaj faktur..."
                        value={linkingSearch}
                        onChange={(e) => {
                          setLinkingSearch(e.target.value);
                          fetchLinkableInvoices(e.target.value);
                        }}
                        sx={{ mb: 2, bgcolor: "background.paper" }}
                      />

                      <FormControl fullWidth size="small" sx={{ mb: 2 }}>
                        <InputLabel>Typ powiązania</InputLabel>
                        <Select
                          value={relationType}
                          label="Typ powiązania"
                          onChange={(e) =>
                            setRelationType(
                              e.target.value as InvoiceRelationType
                            )
                          }
                          sx={{ bgcolor: "background.paper" }}
                        >
                          {Object.entries(InvoiceRelationTypeLabels).map(
                            ([key, label]) => (
                              <MenuItem key={key} value={key}>
                                {label}
                              </MenuItem>
                            )
                          )}
                        </Select>
                      </FormControl>

                      {linkingLoading ? (
                        <Box display="flex" justifyContent="center" py={2}>
                          <CircularProgress size={24} />
                        </Box>
                      ) : linkableInvoices.length > 0 ? (
                        <TableContainer
                          component={Paper}
                          sx={{ maxHeight: 200, mb: 2 }}
                        >
                          <Table size="small" stickyHeader>
                            <TableHead>
                              <TableRow>
                                <TableCell padding="checkbox" />
                                <TableCell>Numer</TableCell>
                                <TableCell>Data</TableCell>
                                <TableCell align="right">Kwota</TableCell>
                              </TableRow>
                            </TableHead>
                            <TableBody>
                              {linkableInvoices.map((inv) => (
                                <TableRow
                                  key={inv.id}
                                  hover
                                  onClick={() => {
                                    setSelectedLinkIds((prev) =>
                                      prev.includes(inv.id)
                                        ? prev.filter((id) => id !== inv.id)
                                        : [...prev, inv.id]
                                    );
                                  }}
                                  selected={selectedLinkIds.includes(inv.id)}
                                  sx={{ cursor: "pointer" }}
                                >
                                  <TableCell padding="checkbox">
                                    <input
                                      type="checkbox"
                                      checked={selectedLinkIds.includes(inv.id)}
                                      readOnly
                                    />
                                  </TableCell>
                                  <TableCell>
                                    <Typography
                                      variant="body2"
                                      fontWeight={500}
                                    >
                                      {inv.invoiceNumber}
                                    </Typography>
                                    <Typography
                                      variant="caption"
                                      color="text.secondary"
                                    >
                                      {inv.invoiceTypeDescription}
                                    </Typography>
                                  </TableCell>
                                  <TableCell>{inv.invoiceDate}</TableCell>
                                  <TableCell align="right">
                                    {inv.grossAmount.toLocaleString("pl-PL", {
                                      minimumFractionDigits: 2,
                                    })}{" "}
                                    zł
                                  </TableCell>
                                </TableRow>
                              ))}
                            </TableBody>
                          </Table>
                        </TableContainer>
                      ) : (
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ mb: 2 }}
                        >
                          Brak faktur do powiązania
                        </Typography>
                      )}

                      <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap" }}>
                        <Button
                          variant="contained"
                          color="primary"
                          size="small"
                          onClick={handleLinkInvoices}
                          disabled={saving || selectedLinkIds.length === 0}
                        >
                          Powiąż ({selectedLinkIds.length})
                        </Button>
                        <Button
                          variant="outlined"
                          size="small"
                          onClick={handleAcceptNoLinking}
                          disabled={saving}
                        >
                          Akceptuję brak powiązania
                        </Button>
                        <Button
                          variant="text"
                          size="small"
                          onClick={handlePostponeLinking}
                          disabled={saving}
                        >
                          Przypomnij później
                        </Button>
                      </Box>
                    </Box>
                  </>
                )}

                <Divider sx={{ my: 2 }} />

                {/* Edit form fields */}
                <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
                  <FormControl fullWidth size="small">
                    <InputLabel>Moduł</InputLabel>
                    <Select
                      value={editForm.moduleType}
                      label="Moduł"
                      onChange={(e) => {
                        const newModuleType = e.target.value;
                        handleFormChange("moduleType", newModuleType);
                        // Show module form for entity-creating modules
                        if (
                          newModuleType === ModuleType.Feeds ||
                          newModuleType === ModuleType.Gas ||
                          newModuleType === ModuleType.ProductionExpenses ||
                          newModuleType === ModuleType.Sales
                        ) {
                          setPendingModuleType(newModuleType);
                          setShowModuleForm(true);
                        } else {
                          setShowModuleForm(false);
                          setPendingModuleType(null);
                        }
                      }}
                    >
                      {Object.entries(ModuleTypeLabels).map(([key, label]) => (
                        <MenuItem key={key} value={key}>
                          {label}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>

                  {/* Module Entity Form */}
                  {showModuleForm && pendingModuleType && details && (
                    <ModuleEntityForm
                      ref={moduleFormRef}
                      invoiceId={details.id}
                      moduleType={pendingModuleType}
                      invoiceData={{
                        invoiceNumber: details.invoiceNumber,
                        invoiceDate: details.invoiceDate,
                        dueDate: details.paymentDueDate || undefined,
                        sellerName: details.sellerName,
                        sellerNip: details.sellerNip,
                        buyerName: details.buyerName,
                        buyerNip: details.buyerNip,
                        grossAmount: details.grossAmount,
                        netAmount: details.netAmount,
                        vatAmount: details.vatAmount,
                        lineItems: parsedXml?.lineItems || [],
                        footer: parsedXml?.footer,
                      }}
                      farms={farms}
                      selectedFarmId={editForm.farmId}
                      selectedCycleId={editForm.cycleId}
                      comment={editForm.comment}
                      mode="accept"
                      onSuccess={() => {
                        setShowModuleForm(false);
                        setPendingModuleType(null);
                        setDetails((prev) =>
                          prev
                            ? { ...prev, status: KSeFInvoiceStatus.Accepted }
                            : null
                        );
                        onSave?.();
                      }}
                    />
                  )}

                  <FormControl fullWidth size="small">
                    <InputLabel>Lokalizacja (Ferma)</InputLabel>
                    <Select
                      value={editForm.farmId}
                      label="Lokalizacja (Ferma)"
                      onChange={(e) =>
                        handleFormChange("farmId", e.target.value)
                      }
                    >
                      <MenuItem value="">
                        <em>Brak</em>
                      </MenuItem>
                      {farms.map((farm) => (
                        <MenuItem key={farm.id} value={farm.id}>
                          {farm.name}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>

                  <FormControl
                    fullWidth
                    size="small"
                    disabled={!editForm.farmId}
                  >
                    <InputLabel>Cykl</InputLabel>
                    <Select
                      value={editForm.cycleId}
                      label="Cykl"
                      onChange={(e) =>
                        handleFormChange("cycleId", e.target.value)
                      }
                    >
                      <MenuItem value="">
                        <em>Brak</em>
                      </MenuItem>
                      {cycles.map((cycle) => (
                        <MenuItem key={cycle.id} value={cycle.id}>
                          {cycle.identifier}/{cycle.year}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>

                  <FormControl fullWidth size="small">
                    <InputLabel>Typ odliczenia VAT</InputLabel>
                    <Select
                      value={editForm.vatDeductionType}
                      label="Typ odliczenia VAT"
                      onChange={(e) =>
                        handleFormChange("vatDeductionType", e.target.value)
                      }
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

                  <FormControl fullWidth size="small">
                    <InputLabel>Status płatności</InputLabel>
                    <Select
                      value={editForm.paymentStatus}
                      label="Status płatności"
                      onChange={(e) =>
                        handleFormChange("paymentStatus", e.target.value)
                      }
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

                  <FormControl fullWidth size="small">
                    <InputLabel>Przypisany pracownik</InputLabel>
                    <Select
                      value={editForm.assignedUserId}
                      label="Przypisany pracownik"
                      onChange={(e) =>
                        handleFormChange("assignedUserId", e.target.value)
                      }
                    >
                      <MenuItem value="">
                        <em>Brak</em>
                      </MenuItem>
                      {users.map((user) => (
                        <MenuItem key={user.id} value={user.id}>
                          {user.name || user.login}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>

                  <TextField
                    fullWidth
                    size="small"
                    label="Powiązana faktura"
                    placeholder="Numer powiązanej faktury"
                    value={editForm.relatedInvoiceNumber}
                    onChange={(e) =>
                      handleFormChange("relatedInvoiceNumber", e.target.value)
                    }
                  />

                  <TextField
                    fullWidth
                    size="small"
                    label="Komentarz"
                    multiline
                    rows={3}
                    value={editForm.comment}
                    onChange={(e) =>
                      handleFormChange("comment", e.target.value)
                    }
                  />
                </Box>
              </Box>
            </Grid>
          </Grid>
        ) : (
          <Typography
            color="text.secondary"
            sx={{ py: 4, textAlign: "center" }}
          >
            Brak danych do wyświetlenia
          </Typography>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} variant="contained">
          Zamknij
        </Button>
      </DialogActions>

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
            Przypisz fakturę do innego pracownika bez zmiany jej statusu.
            Faktura pozostanie jako "Nowa" do weryfikacji.
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
    </AppDialog>
  );
};

export default InvoiceDetailsModal;
