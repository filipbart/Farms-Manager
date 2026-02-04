import type { FilterConfig } from "../../components/filters/filter-types";
import type { KSeFInvoicesFilters } from "../../models/accounting/ksef-filters";
import {
  KSeFInvoiceStatusLabels,
  KSeFPaymentStatusLabels,
  InvoiceSourceLabels,
  ModuleTypeLabels,
} from "../../models/accounting/ksef-invoice";

interface AccountingFiltersConfigOptions {
  users?: { value: string; label: string }[];
  farms?: { value: string; label: string }[];
}

export const getAccountingFiltersConfig = (
  options?: AccountingFiltersConfigOptions,
): FilterConfig<keyof KSeFInvoicesFilters>[] => [
  {
    key: "buyerName",
    label: "Nabywca",
    type: "text",
  },
  {
    key: "sellerName",
    label: "Sprzedawca",
    type: "text",
  },
  {
    key: "invoiceNumber",
    label: "Numer faktury",
    type: "text",
  },
  {
    key: "source",
    label: "Źródło faktury",
    type: "select",
    options: Object.entries(InvoiceSourceLabels).map(([value, label]) => ({
      value,
      label,
    })),
  },
  {
    key: "invoiceDateFrom",
    label: "Data wystawienia od",
    type: "date",
  },
  {
    key: "invoiceDateTo",
    label: "Data wystawienia do",
    type: "date",
  },
  {
    key: "paymentDueDateFrom",
    label: "Termin płatności od",
    type: "date",
  },
  {
    key: "paymentDueDateTo",
    label: "Termin płatności do",
    type: "date",
  },
  {
    key: "status",
    label: "Status faktury",
    type: "select",
    options: Object.entries(KSeFInvoiceStatusLabels).map(([value, label]) => ({
      value,
      label,
    })),
  },
  {
    key: "paymentStatus",
    label: "Status płatności",
    type: "multiSelect",
    options: Object.entries(KSeFPaymentStatusLabels).map(([value, label]) => ({
      value,
      label,
    })),
  },
  {
    key: "assignedUserId",
    label: "Przypisany pracownik",
    type: "select",
    options: options?.users ?? [],
  },
  {
    key: "moduleType",
    label: "Moduł",
    type: "select",
    options: Object.entries(ModuleTypeLabels).map(([value, label]) => ({
      value,
      label,
    })),
  },
  {
    key: "farmId",
    label: "Lokalizacja",
    type: "select",
    options: options?.farms ?? [],
  },
  {
    key: "exclusions",
    label: "Wykluczenia",
    type: "text",
  },
  {
    key: "unpaidDaysAhead",
    label: "Nieopłacone do X dni",
    type: "number",
  },
];
