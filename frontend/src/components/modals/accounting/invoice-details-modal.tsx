import React, { useEffect, useState, useMemo } from "react";
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
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { toast } from "react-toastify";
import AppDialog from "../../common/app-dialog";
import { AccountingService } from "../../../services/accounting-service";
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
  KSeFInvoiceStatus,
  KSeFPaymentStatus,
} from "../../../models/accounting/ksef-invoice";
import dayjs from "dayjs";
import { parseKSeFInvoiceXml } from "../../../utils/ksef-xml-parser";

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

const InvoiceDetailsModal: React.FC<InvoiceDetailsModalProps> = ({
  open,
  onClose,
  invoice,
}) => {
  const [loading, setLoading] = useState(false);
  const [details, setDetails] = useState<KSeFInvoiceDetails | null>(null);

  const parsedXml = useMemo(() => {
    if (!details?.invoiceXml) return null;
    return parseKSeFInvoiceXml(details.invoiceXml);
  }, [details?.invoiceXml]);

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
                    <DetailRow label="NIP" value={parsedXml.thirdParty.nip} />
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
                  <Typography variant="subtitle1" fontWeight={600} gutterBottom>
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
                        {parsedXml.invoiceData.vatBreakdown.map((vat, idx) => (
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
                        ))}
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
                                {item.quantity?.toLocaleString("pl-PL") || "—"}
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
                <Typography variant="subtitle1" fontWeight={600} gutterBottom>
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
                      <Typography variant="body2" fontWeight={500} gutterBottom>
                        Rachunki bankowe:
                      </Typography>
                      {parsedXml.payment.bankAccounts.map((account, idx) => (
                        <Box key={idx} sx={{ pl: 2, mb: 1 }}>
                          <DetailRow
                            label="Nr rachunku"
                            value={account.accountNumber}
                          />
                          {account.bankName && (
                            <DetailRow label="Bank" value={account.bankName} />
                          )}
                          {account.description && (
                            <DetailRow
                              label="Opis"
                              value={account.description}
                            />
                          )}
                        </Box>
                      ))}
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

            {parsedXml?.footer && (
              <>
                <Divider sx={{ my: 2 }} />
                <Typography variant="subtitle1" fontWeight={600} gutterBottom>
                  Stopka faktury
                </Typography>
                <Box sx={{ pl: 2 }}>
                  <Typography variant="body2" sx={{ whiteSpace: "pre-wrap" }}>
                    {parsedXml.footer}
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
