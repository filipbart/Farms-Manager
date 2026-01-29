import { useState } from "react";
import type { FC } from "react";
import {
  Box,
  Grid,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Typography,
  Chip,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs, { Dayjs } from "dayjs";
import type { KSeFInvoicesFilters } from "../../models/accounting/ksef-filters";
import {
  KSeFInvoiceStatusLabels,
  KSeFPaymentStatusLabels,
  InvoiceSourceLabels,
  ModuleTypeLabels,
  KSeFPaymentStatus,
} from "../../models/accounting/ksef-invoice";

type FilterValue = KSeFInvoicesFilters[keyof KSeFInvoicesFilters];

interface AccountingFiltersProps {
  filters: KSeFInvoicesFilters;
  dispatch: React.Dispatch<
    | { type: "set"; key: keyof KSeFInvoicesFilters; value: FilterValue }
    | { type: "setMultiple"; payload: Partial<KSeFInvoicesFilters> }
  >;
  users?: { value: string; label: string }[];
  farms?: { value: string; label: string }[];
}

const AccountingFilters: FC<AccountingFiltersProps> = ({
  filters,
  dispatch,
  users = [],
  farms = [],
}) => {
  const [advancedExpanded, setAdvancedExpanded] = useState(false);
  const invoiceStatusEntries = Object.entries(KSeFInvoiceStatusLabels) as Array<
    [string, string]
  >;
  const paymentStatusEntries = Object.entries(KSeFPaymentStatusLabels) as Array<
    [string, string]
  >;
  const sourceEntries = Object.entries(InvoiceSourceLabels) as Array<
    [string, string]
  >;
  const moduleEntries = Object.entries(ModuleTypeLabels) as Array<
    [string, string]
  >;

  const handleFilterChange = (
    key: keyof KSeFInvoicesFilters,
    value: FilterValue,
  ) => {
    dispatch({
      type: "setMultiple",
      payload: { [key]: value },
    });
  };

  const handleDateChange = (
    key: keyof KSeFInvoicesFilters,
    date: Dayjs | null,
  ) => {
    const value = date ? date.format("YYYY-MM-DD") : "";
    handleFilterChange(key, value);
  };

  const handlePaymentStatusChange = (value: string) => {
    const currentValues = filters.paymentStatus || [];
    const statusValue = value as KSeFPaymentStatus;
    if (currentValues.includes(statusValue)) {
      // Remove value
      const newValues = currentValues.filter((v) => v !== statusValue);
      handleFilterChange("paymentStatus", newValues);
    } else {
      // Add value
      const newValues = [...currentValues, statusValue];
      handleFilterChange("paymentStatus", newValues);
    }
  };

  const isPaymentStatusSelected = (value: string) => {
    const currentValues = filters.paymentStatus || [];
    return currentValues.includes(value as KSeFPaymentStatus);
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Box sx={{ mb: 2 }}>
        {/* First row: Nabywca, Sprzedawca, Numer faktury, Lokalizacja */}
        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <TextField
              fullWidth
              size="small"
              label="Nabywca"
              value={filters.buyerName || ""}
              onChange={(e) => handleFilterChange("buyerName", e.target.value)}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <TextField
              fullWidth
              size="small"
              label="Sprzedawca"
              value={filters.sellerName || ""}
              onChange={(e) => handleFilterChange("sellerName", e.target.value)}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <TextField
              fullWidth
              size="small"
              label="Numer faktury"
              value={filters.invoiceNumber || ""}
              onChange={(e) =>
                handleFilterChange("invoiceNumber", e.target.value)
              }
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormControl fullWidth size="small">
              <InputLabel>Lokalizacja</InputLabel>
              <Select
                value={filters.farmId || ""}
                label="Lokalizacja"
                onChange={(e) => handleFilterChange("farmId", e.target.value)}
              >
                <MenuItem value="">Wszystkie</MenuItem>
                {farms.map((farm) => (
                  <MenuItem key={farm.value} value={farm.value}>
                    {farm.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
        </Grid>

        {/* Second row: Status faktury, Przypisany pracownik, Status płatności */}
        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormControl fullWidth size="small">
              <InputLabel>Status faktury</InputLabel>
              <Select
                value={filters.status || ""}
                label="Status faktury"
                onChange={(e) => handleFilterChange("status", e.target.value)}
              >
                <MenuItem value="">Wszystkie</MenuItem>
                {invoiceStatusEntries.map(([value, label]) => (
                  <MenuItem key={value} value={value}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormControl fullWidth size="small">
              <InputLabel>Przypisany pracownik</InputLabel>
              <Select
                value={filters.assignedUserId || ""}
                label="Przypisany pracownik"
                onChange={(e) =>
                  handleFilterChange("assignedUserId", e.target.value)
                }
              >
                <MenuItem value="">Wszyscy</MenuItem>
                {users.map((user) => (
                  <MenuItem key={user.value} value={user.value}>
                    {user.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid size={{ xs: 12, sm: 12, md: 4 }}>
            <Box>
              <Typography
                variant="caption"
                color="text.secondary"
                sx={{ mb: 1, display: "block" }}
              >
                Status płatności
              </Typography>
              <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1 }}>
                {paymentStatusEntries.map(([value, label]) => (
                  <Chip
                    key={value}
                    label={label}
                    size="small"
                    clickable
                    variant={
                      isPaymentStatusSelected(value) ? "filled" : "outlined"
                    }
                    color={
                      isPaymentStatusSelected(value) ? "primary" : "default"
                    }
                    onClick={() => handlePaymentStatusChange(value)}
                  />
                ))}
              </Box>
            </Box>
          </Grid>
        </Grid>

        {/* Advanced Filters Accordion */}
        <Accordion
          expanded={advancedExpanded}
          onChange={() => setAdvancedExpanded(!advancedExpanded)}
        >
          <AccordionSummary expandIcon={<ExpandMoreIcon />}>
            <Typography variant="subtitle2">Zaawansowane filtry</Typography>
          </AccordionSummary>
          <AccordionDetails>
            <Grid container spacing={2}>
              {/* First advanced row: Data wystawienia od, Data wystawienia do */}
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <DatePicker
                  label="Data wystawienia od"
                  value={
                    filters.invoiceDateFrom
                      ? dayjs(filters.invoiceDateFrom)
                      : null
                  }
                  onChange={(date) => handleDateChange("invoiceDateFrom", date)}
                  slotProps={{ textField: { fullWidth: true, size: "small" } }}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <DatePicker
                  label="Data wystawienia do"
                  value={
                    filters.invoiceDateTo ? dayjs(filters.invoiceDateTo) : null
                  }
                  onChange={(date) => handleDateChange("invoiceDateTo", date)}
                  slotProps={{ textField: { fullWidth: true, size: "small" } }}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <DatePicker
                  label="Termin płatności od"
                  value={
                    filters.paymentDueDateFrom
                      ? dayjs(filters.paymentDueDateFrom)
                      : null
                  }
                  onChange={(date) =>
                    handleDateChange("paymentDueDateFrom", date)
                  }
                  slotProps={{ textField: { fullWidth: true, size: "small" } }}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <DatePicker
                  label="Termin płatności do"
                  value={
                    filters.paymentDueDateTo
                      ? dayjs(filters.paymentDueDateTo)
                      : null
                  }
                  onChange={(date) =>
                    handleDateChange("paymentDueDateTo", date)
                  }
                  slotProps={{ textField: { fullWidth: true, size: "small" } }}
                />
              </Grid>

              {/* Second advanced row: Źródło faktury, Moduł */}
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormControl fullWidth size="small">
                  <InputLabel>Źródło faktury</InputLabel>
                  <Select
                    value={filters.source || ""}
                    label="Źródło faktury"
                    onChange={(e) =>
                      handleFilterChange("source", e.target.value)
                    }
                  >
                    <MenuItem value="">Wszystkie</MenuItem>
                    {sourceEntries.map(([value, label]) => (
                      <MenuItem key={value} value={value}>
                        {label}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormControl fullWidth size="small">
                  <InputLabel>Moduł</InputLabel>
                  <Select
                    value={filters.moduleType || ""}
                    label="Moduł"
                    onChange={(e) =>
                      handleFilterChange("moduleType", e.target.value)
                    }
                  >
                    <MenuItem value="">Wszystkie</MenuItem>
                    {moduleEntries.map(([value, label]) => (
                      <MenuItem key={value} value={value}>
                        {label}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
            </Grid>
          </AccordionDetails>
        </Accordion>
      </Box>
    </LocalizationProvider>
  );
};

export default AccountingFilters;
