import React, { useEffect, useState } from "react";
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
} from "@mui/material";
import { toast } from "react-toastify";
import AppDialog from "../../common/app-dialog";
import { AccountingService } from "../../../services/accounting-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type {
  KSeFInvoiceDetails,
  KSeFInvoiceListModel,
} from "../../../models/accounting/ksef-invoice";
import {
  KSeFInvoiceStatusLabels,
  KSeFPaymentStatusLabels,
  KSeFInvoicePaymentTypeLabels,
  InvoiceSourceLabels,
  ModuleTypeLabels,
  KSeFInvoiceTypeLabels,
  KSeFInvoiceStatus,
  KSeFPaymentStatus,
} from "../../../models/accounting/ksef-invoice";
import dayjs from "dayjs";

interface InvoiceDetailsModalProps {
  open: boolean;
  onClose: () => void;
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

const InvoiceDetailsModal: React.FC<InvoiceDetailsModalProps> = ({
  open,
  onClose,
  invoice,
}) => {
  const [loading, setLoading] = useState(false);
  const [details, setDetails] = useState<KSeFInvoiceDetails | null>(null);

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
    <AppDialog open={open} onClose={onClose} maxWidth="md" fullWidth>
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
          <Box sx={{ mt: 2 }}>
            {/* Sekcja: Podstawowe informacje */}
            <Typography variant="subtitle1" fontWeight={600} gutterBottom>
              Podstawowe informacje
            </Typography>
            <Box sx={{ mb: 3, pl: 2 }}>
              <DetailRow label="Numer faktury" value={details.invoiceNumber} />
              <DetailRow label="Numer KSeF" value={details.kSeFNumber || "—"} />
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
                    color={details.source === "KSeF" ? "primary" : "default"}
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
            <Typography variant="subtitle1" fontWeight={600} gutterBottom>
              Strony transakcji
            </Typography>
            <Box sx={{ mb: 3, pl: 2 }}>
              <DetailRow label="Sprzedawca" value={details.sellerName} />
              <DetailRow label="NIP sprzedawcy" value={details.sellerNip} />
              <DetailRow label="Nabywca" value={details.buyerName} />
              <DetailRow label="NIP nabywcy" value={details.buyerNip} />
            </Box>

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
            </Box>

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
                      KSeFInvoiceStatusLabels[details.status] || details.status
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
                    ? ModuleTypeLabels[details.moduleType] || details.moduleType
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
                <Typography variant="subtitle1" fontWeight={600} gutterBottom>
                  Komentarz
                </Typography>
                <Box sx={{ pl: 2 }}>
                  <Typography variant="body2" sx={{ whiteSpace: "pre-wrap" }}>
                    {details.comment}
                  </Typography>
                </Box>
              </>
            )}

            <Divider sx={{ my: 2 }} />

            {/* Sekcja: Metadane */}
            <Typography variant="subtitle1" fontWeight={600} gutterBottom>
              Metadane
            </Typography>
            <Box sx={{ pl: 2 }}>
              <DetailRow
                label="Data utworzenia"
                value={
                  details.createdAt
                    ? dayjs(details.createdAt).format("YYYY-MM-DD HH:mm")
                    : "—"
                }
              />
              <DetailRow label="Utworzono przez" value={details.createdBy} />
            </Box>
          </Box>
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
