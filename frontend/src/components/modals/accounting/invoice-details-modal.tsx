import React, { useEffect, useState, useMemo, useCallback } from "react";
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
} from "../../../models/accounting/ksef-invoice";
import {
  KSeFInvoiceStatusLabels,
  KSeFPaymentStatusLabels,
  KSeFInvoicePaymentTypeLabels,
  InvoiceSourceLabels,
  ModuleTypeLabels,
  KSeFInvoiceTypeLabels,
  VatDeductionTypeLabels,
  KSeFInvoiceStatus,
  KSeFPaymentStatus,
  ModuleType,
  VatDeductionType,
} from "../../../models/accounting/ksef-invoice";
import type FarmRowModel from "../../../models/farms/farm-row-model";
import type CycleDto from "../../../models/farms/latest-cycle";
import type { UserListModel } from "../../../models/users/users";
import dayjs from "dayjs";
import { parseKSeFInvoiceXml } from "../../../utils/ksef-xml-parser";

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

const PartySection: React.FC<{
  title: string;
  party: KSeFPartyData | undefined;
  fallbackName?: string;
  fallbackNip?: string;
}> = ({ title, party, fallbackName, fallbackNip }) => (
  <Accordion defaultExpanded sx={{ mb: 1 }}>
    <AccordionSummary expandIcon={<ExpandMoreIcon />}>
      <Typography fontWeight={600}>{title}</Typography>
    </AccordionSummary>
    <AccordionDetails>
      <Box sx={{ pl: 1 }}>
        <DetailRow label="Nazwa" value={party?.name || fallbackName} />
        <DetailRow label="NIP" value={party?.nip || fallbackNip} />
        {party?.vatEuNumber && (
          <DetailRow label="NIP UE" value={party.vatEuNumber} />
        )}
        {party?.idNumber && (
          <DetailRow label="Nr identyfikacyjny" value={party.idNumber} />
        )}
        <DetailRow label="Adres" value={formatAddress(party)} />
        {party?.address?.gln && (
          <DetailRow label="GLN" value={party.address.gln} />
        )}
        {party?.contact?.email && (
          <DetailRow label="Email" value={party.contact.email} />
        )}
        {party?.contact?.phone && (
          <DetailRow label="Telefon" value={party.contact.phone} />
        )}
      </Box>
    </AccordionDetails>
  </Accordion>
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

  const parsedXml = useMemo(() => {
    if (!details?.invoiceXml) return null;
    return parseKSeFInvoiceXml(details.invoiceXml);
  }, [details?.invoiceXml]);

  // Initialize form when details are loaded
  useEffect(() => {
    if (details) {
      setEditForm({
        moduleType: details.moduleType || ModuleType.None,
        vatDeductionType: details.vatDeductionType || VatDeductionType.Full,
        paymentStatus: details.paymentStatus || KSeFPaymentStatus.Unpaid,
        farmId: details.farmId || "",
        cycleId: details.cycleId || "",
        assignedUserId: details.assignedUserId || "",
        comment: details.comment || "",
        relatedInvoiceNumber: details.relatedInvoiceNumber || "",
      });
    }
  }, [details]);

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
      // Clear cycle when farm changes
      if (field === "farmId") {
        setEditForm((prev) => ({ ...prev, cycleId: "" }));
      }
    },
    []
  );

  const handleSave = useCallback(async () => {
    if (!details) return;
    setSaving(true);
    try {
      await handleApiResponse(
        () =>
          AccountingService.updateInvoice(details.id, {
            moduleType: editForm.moduleType || undefined,
            vatDeductionType: editForm.vatDeductionType,
            paymentStatus: editForm.paymentStatus,
            farmId: editForm.farmId || null,
            cycleId: editForm.cycleId || null,
            assignedUserId: editForm.assignedUserId || null,
            comment: editForm.comment,
            relatedInvoiceNumber: editForm.relatedInvoiceNumber,
          }),
        () => {
          toast.success("Faktura została zaktualizowana");
          onSave?.();
        },
        undefined,
        "Błąd podczas zapisywania faktury"
      );
    } finally {
      setSaving(false);
    }
  }, [details, editForm, onSave]);

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
            {/* Left side - Invoice visualization */}
            <Grid size={{ xs: 12, md: 7 }}>
              <Box
                sx={{
                  pr: { md: 2 },
                  borderRight: { md: 1 },
                  borderColor: { md: "divider" },
                }}
              >
                {/* Sekcja: Podstawowe informacje */}
                <Typography variant="subtitle1" fontWeight={600} gutterBottom>
                  Podstawowe informacje
                </Typography>
                <Box sx={{ mb: 3, pl: 2 }}>
                  <DetailRow
                    label="Numer faktury"
                    value={details.invoiceNumber}
                  />
                  <DetailRow
                    label="Numer KSeF"
                    value={details.kSeFNumber || "—"}
                  />
                  <DetailRow
                    label="Data wystawienia"
                    value={
                      details.invoiceDate
                        ? dayjs(details.invoiceDate).format("YYYY-MM-DD")
                        : "—"
                    }
                  />
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
                          InvoiceSourceLabels[details.source] || details.source
                        }
                        size="small"
                        color={
                          details.source === "KSeF" ? "primary" : "default"
                        }
                        variant="outlined"
                      />
                    }
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

                <Divider sx={{ my: 2 }} />

                {/* Sekcja: Strony transakcji */}
                <Typography
                  variant="subtitle1"
                  fontWeight={600}
                  gutterBottom
                  sx={{ mb: 2 }}
                >
                  Strony transakcji
                </Typography>
                <PartySection
                  title="Sprzedawca"
                  party={parsedXml?.seller}
                  fallbackName={details.sellerName}
                  fallbackNip={details.sellerNip}
                />
                <PartySection
                  title="Nabywca"
                  party={parsedXml?.buyer}
                  fallbackName={details.buyerName}
                  fallbackNip={details.buyerNip}
                />
                {parsedXml?.thirdParty && (
                  <Accordion sx={{ mb: 1 }}>
                    <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                      <Typography fontWeight={600}>
                        Podmiot trzeci{" "}
                        {parsedXml.thirdParty.role
                          ? `(${parsedXml.thirdParty.role})`
                          : ""}
                      </Typography>
                    </AccordionSummary>
                    <AccordionDetails>
                      <Box sx={{ pl: 1 }}>
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
                    </AccordionDetails>
                  </Accordion>
                )}

                <Divider sx={{ my: 2 }} />

                {/* Sekcja: Kwoty */}
                <Typography variant="subtitle1" fontWeight={600} gutterBottom>
                  Kwoty
                </Typography>
                <Box sx={{ mb: 3, pl: 2 }}>
                  <DetailRow
                    label="Kwota brutto"
                    value={formatCurrency(details.grossAmount)}
                  />
                  <DetailRow
                    label="Kwota netto"
                    value={formatCurrency(details.netAmount)}
                  />
                  <DetailRow
                    label="Kwota VAT"
                    value={formatCurrency(details.vatAmount)}
                  />
                  {parsedXml?.invoiceData?.currency && (
                    <DetailRow
                      label="Waluta"
                      value={parsedXml.invoiceData.currency}
                    />
                  )}
                  {parsedXml?.invoiceData?.saleDate && (
                    <DetailRow
                      label="Data sprzedaży"
                      value={dayjs(parsedXml.invoiceData.saleDate).format(
                        "YYYY-MM-DD"
                      )}
                    />
                  )}
                  {parsedXml?.invoiceData?.issuePlace && (
                    <DetailRow
                      label="Miejsce wystawienia"
                      value={parsedXml.invoiceData.issuePlace}
                    />
                  )}
                </Box>

                {/* Sekcja: Rozbicie VAT */}
                {parsedXml?.invoiceData?.vatBreakdown &&
                  parsedXml.invoiceData.vatBreakdown.length > 0 && (
                    <>
                      <Divider sx={{ my: 2 }} />
                      <Typography
                        variant="subtitle1"
                        fontWeight={600}
                        gutterBottom
                      >
                        Rozbicie VAT
                      </Typography>
                      <TableContainer
                        component={Paper}
                        variant="outlined"
                        sx={{ mb: 2 }}
                      >
                        <Table size="small">
                          <TableHead>
                            <TableRow>
                              <TableCell>Stawka</TableCell>
                              <TableCell align="right">Netto</TableCell>
                              <TableCell align="right">VAT</TableCell>
                            </TableRow>
                          </TableHead>
                          <TableBody>
                            {parsedXml.invoiceData.vatBreakdown.map(
                              (vat, idx) => (
                                <TableRow key={idx}>
                                  <TableCell>{vat.rate}</TableCell>
                                  <TableCell align="right">
                                    {formatCurrency(vat.netAmount)}
                                  </TableCell>
                                  <TableCell align="right">
                                    {vat.vatAmount !== undefined
                                      ? formatCurrency(vat.vatAmount)
                                      : "—"}
                                  </TableCell>
                                </TableRow>
                              )
                            )}
                          </TableBody>
                        </Table>
                      </TableContainer>
                    </>
                  )}

                {/* Sekcja: Pozycje faktury */}
                {parsedXml?.lineItems && parsedXml.lineItems.length > 0 && (
                  <>
                    <Divider sx={{ my: 2 }} />
                    <Accordion>
                      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                        <Typography fontWeight={600}>
                          Pozycje faktury ({parsedXml.lineItems.length})
                        </Typography>
                      </AccordionSummary>
                      <AccordionDetails>
                        <TableContainer component={Paper} variant="outlined">
                          <Table size="small">
                            <TableHead>
                              <TableRow>
                                <TableCell>Lp.</TableCell>
                                <TableCell>Nazwa</TableCell>
                                <TableCell align="right">Ilość</TableCell>
                                <TableCell>J.m.</TableCell>
                                <TableCell align="right">Cena netto</TableCell>
                                <TableCell align="right">Netto</TableCell>
                                <TableCell align="right">VAT %</TableCell>
                                <TableCell align="right">Brutto</TableCell>
                              </TableRow>
                            </TableHead>
                            <TableBody>
                              {parsedXml.lineItems.map((item) => (
                                <TableRow key={item.lineNumber}>
                                  <TableCell>{item.lineNumber}</TableCell>
                                  <TableCell sx={{ maxWidth: 200 }}>
                                    <Typography
                                      variant="body2"
                                      noWrap
                                      title={item.name}
                                    >
                                      {item.name || "—"}
                                    </Typography>
                                    {(item.pkwiu || item.gtu) && (
                                      <Typography
                                        variant="caption"
                                        color="text.secondary"
                                      >
                                        {item.pkwiu && `PKWiU: ${item.pkwiu}`}
                                        {item.pkwiu && item.gtu && " | "}
                                        {item.gtu && `GTU: ${item.gtu}`}
                                      </Typography>
                                    )}
                                  </TableCell>
                                  <TableCell align="right">
                                    {item.quantity?.toLocaleString("pl-PL") ||
                                      "—"}
                                  </TableCell>
                                  <TableCell>{item.unit || "—"}</TableCell>
                                  <TableCell align="right">
                                    {formatCurrency(item.unitPriceNet)}
                                  </TableCell>
                                  <TableCell align="right">
                                    {formatCurrency(item.netAmount)}
                                  </TableCell>
                                  <TableCell align="right">
                                    {item.vatRate !== undefined
                                      ? `${item.vatRate}%`
                                      : "—"}
                                  </TableCell>
                                  <TableCell align="right">
                                    {formatCurrency(item.grossAmount)}
                                  </TableCell>
                                </TableRow>
                              ))}
                            </TableBody>
                          </Table>
                        </TableContainer>
                      </AccordionDetails>
                    </Accordion>
                  </>
                )}

                {/* Sekcja: Płatność */}
                {parsedXml?.payment && (
                  <>
                    <Divider sx={{ my: 2 }} />
                    <Typography
                      variant="subtitle1"
                      fontWeight={600}
                      gutterBottom
                    >
                      Dane płatności z XML
                    </Typography>
                    <Box sx={{ mb: 3, pl: 2 }}>
                      {parsedXml.payment.paymentMethod && (
                        <DetailRow
                          label="Forma płatności"
                          value={parsedXml.payment.paymentMethod}
                        />
                      )}
                      {parsedXml.payment.dueDate && (
                        <DetailRow
                          label="Termin płatności"
                          value={dayjs(parsedXml.payment.dueDate).format(
                            "YYYY-MM-DD"
                          )}
                        />
                      )}
                      {parsedXml.payment.isPaid !== undefined && (
                        <DetailRow
                          label="Status zapłaty (XML)"
                          value={
                            <Chip
                              label={
                                parsedXml.payment.isPaid
                                  ? "Zapłacono"
                                  : "Niezapłacono"
                              }
                              size="small"
                              color={
                                parsedXml.payment.isPaid ? "success" : "default"
                              }
                              variant="outlined"
                            />
                          }
                        />
                      )}
                      {parsedXml.payment.paymentDate && (
                        <DetailRow
                          label="Data zapłaty"
                          value={dayjs(parsedXml.payment.paymentDate).format(
                            "YYYY-MM-DD"
                          )}
                        />
                      )}
                      {parsedXml.payment.paymentDescription && (
                        <DetailRow
                          label="Opis płatności"
                          value={parsedXml.payment.paymentDescription}
                        />
                      )}
                    </Box>

                    {/* Rachunki bankowe */}
                    {parsedXml.payment.bankAccounts &&
                      parsedXml.payment.bankAccounts.length > 0 && (
                        <Box sx={{ pl: 2, mb: 2 }}>
                          <Typography
                            variant="body2"
                            fontWeight={500}
                            gutterBottom
                          >
                            Rachunki bankowe:
                          </Typography>
                          {parsedXml.payment.bankAccounts.map(
                            (account, idx) => (
                              <Box key={idx} sx={{ pl: 2, mb: 1 }}>
                                <DetailRow
                                  label="Nr rachunku"
                                  value={account.accountNumber}
                                />
                                {account.bankName && (
                                  <DetailRow
                                    label="Bank"
                                    value={account.bankName}
                                  />
                                )}
                                {account.description && (
                                  <DetailRow
                                    label="Opis"
                                    value={account.description}
                                  />
                                )}
                              </Box>
                            )
                          )}
                        </Box>
                      )}
                  </>
                )}

                <Divider sx={{ my: 2 }} />

                {/* Sekcja: Statusy */}
                <Typography variant="subtitle1" fontWeight={600} gutterBottom>
                  Statusy i płatności
                </Typography>
                <Box sx={{ mb: 3, pl: 2 }}>
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
                          KSeFPaymentStatusLabels[details.paymentStatus] ||
                          details.paymentStatus
                        }
                        size="small"
                        color={getPaymentStatusColor(details.paymentStatus)}
                        variant="outlined"
                      />
                    }
                  />
                  <DetailRow
                    label="Typ płatności"
                    value={
                      KSeFInvoicePaymentTypeLabels[details.paymentType] ||
                      details.paymentType
                    }
                  />
                </Box>

                <Divider sx={{ my: 2 }} />

                {/* Sekcja: Przypisania */}
                <Typography variant="subtitle1" fontWeight={600} gutterBottom>
                  Przypisania i dodatkowe
                </Typography>
                <Box sx={{ mb: 3, pl: 2 }}>
                  <DetailRow
                    label="Moduł"
                    value={
                      details.moduleType
                        ? ModuleTypeLabels[details.moduleType] ||
                          details.moduleType
                        : "—"
                    }
                  />
                  <DetailRow label="Lokalizacja" value={details.location} />
                  <DetailRow
                    label="Przypisany użytkownik"
                    value={details.assignedUserName}
                  />
                  <DetailRow
                    label="Powiązana faktura"
                    value={details.relatedInvoiceNumber}
                  />
                </Box>

                {details.comment && (
                  <>
                    <Divider sx={{ my: 2 }} />
                    <Typography
                      variant="subtitle1"
                      fontWeight={600}
                      gutterBottom
                    >
                      Komentarz
                    </Typography>
                    <Box sx={{ pl: 2 }}>
                      <Typography
                        variant="body2"
                        sx={{ whiteSpace: "pre-wrap" }}
                      >
                        {details.comment}
                      </Typography>
                    </Box>
                  </>
                )}

                {parsedXml?.footer && (
                  <>
                    <Divider sx={{ my: 2 }} />
                    <Typography
                      variant="subtitle1"
                      fontWeight={600}
                      gutterBottom
                    >
                      Stopka faktury
                    </Typography>
                    <Box sx={{ pl: 2 }}>
                      <Typography
                        variant="body2"
                        sx={{ whiteSpace: "pre-wrap" }}
                      >
                        {parsedXml.footer}
                      </Typography>
                    </Box>
                  </>
                )}
              </Box>
            </Grid>

            {/* Right side - Edit panel */}
            <Grid size={{ xs: 12, md: 5 }}>
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
                      onClick={() =>
                        handleStatusChange(KSeFInvoiceStatus.Accepted)
                      }
                      disabled={saving}
                    >
                      Zaakceptuj
                    </Button>
                    <Button
                      color="info"
                      variant={
                        details.status === KSeFInvoiceStatus.SentToOffice
                          ? "contained"
                          : "outlined"
                      }
                      startIcon={<PauseIcon />}
                      onClick={() =>
                        handleStatusChange(KSeFInvoiceStatus.SentToOffice)
                      }
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

                <Divider sx={{ my: 2 }} />

                {/* Edit form fields */}
                <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
                  <FormControl fullWidth size="small">
                    <InputLabel>Moduł</InputLabel>
                    <Select
                      value={editForm.moduleType}
                      label="Moduł"
                      onChange={(e) =>
                        handleFormChange("moduleType", e.target.value)
                      }
                    >
                      {Object.entries(ModuleTypeLabels).map(([key, label]) => (
                        <MenuItem key={key} value={key}>
                          {label}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>

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

                  <Button
                    variant="contained"
                    color="primary"
                    onClick={handleSave}
                    disabled={saving}
                    sx={{ mt: 1 }}
                  >
                    {saving ? <CircularProgress size={24} /> : "Zapisz zmiany"}
                  </Button>
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
    </AppDialog>
  );
};

export default InvoiceDetailsModal;
