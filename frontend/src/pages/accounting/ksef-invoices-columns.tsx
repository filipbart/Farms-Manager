import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { Chip, IconButton, Tooltip } from "@mui/material";
import { MdPictureAsPdf, MdCode, MdDeleteForever } from "react-icons/md";
import type { KSeFInvoiceListModel } from "../../models/accounting/ksef-invoice";
import {
  KSeFPaymentStatus,
  KSeFInvoiceStatusLabels,
  KSeFPaymentStatusLabels,
  KSeFInvoicePaymentTypeLabels,
  InvoiceSourceLabels,
  ModuleTypeLabels,
  KSeFInvoiceTypeLabels,
  VatDeductionTypeLabels,
  KSeFInvoiceStatus,
  VatDeductionType,
} from "../../models/accounting/ksef-invoice";

interface GetKSeFInvoicesColumnsProps {
  onDownloadPdf: (invoice: KSeFInvoiceListModel) => void;
  onDownloadXml: (invoice: KSeFInvoiceListModel) => void;
  onDelete?: (invoice: KSeFInvoiceListModel) => void;
  downloadingId: string | null;
}

const getStatusColor = (
  status: KSeFInvoiceStatus,
  invoiceDate?: string | null,
) => {
  if (status === KSeFInvoiceStatus.New && invoiceDate) {
    const daysSinceIssued = dayjs().diff(dayjs(invoiceDate), "day");
    if (daysSinceIssued >= 15) {
      return "error"; // 🔴 15+ dni – zaległe
    }
    if (daysSinceIssued >= 8) {
      return "warning"; // 🟠 8–14 dni – pilne (używamy warning jako pomarańczowy)
    }
    // 🟡 4–7 dni – bez zmian (info jako żółty) lub default
    if (daysSinceIssued >= 4) {
      return "info";
    }
    return "default"; // < 4 dni – brak koloru
  }

  switch (status) {
    case KSeFInvoiceStatus.Accepted:
      return "success";
    case KSeFInvoiceStatus.Rejected:
      return "error";
    case KSeFInvoiceStatus.New:
    default:
      return "default";
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

export const getKSeFInvoicesColumns = ({
  onDownloadPdf,
  onDownloadXml,
  onDelete,
  downloadingId,
}: GetKSeFInvoicesColumnsProps): GridColDef<KSeFInvoiceListModel>[] => [
  {
    field: "id",
    headerName: "ID",
    width: 100,
    hideable: true,
  },
  {
    field: "kSeFNumber",
    headerName: "ID KSeF",
    width: 180,
    renderCell: (params) => (
      <Tooltip title={params.value || "—"}>
        <span className="truncate">{params.value || "—"}</span>
      </Tooltip>
    ),
  },
  {
    field: "nip",
    headerName: "NIP",
    width: 130,
    valueGetter: (_value, row) => {
      // Dla sprzedaży pokazujemy NIP nabywcy, dla zakupu NIP sprzedawcy
      return row.invoiceType === "Sales" ? row.buyerNip : row.sellerNip;
    },
    valueFormatter: (value: string) => {
      if (!value) return "—";

      // Usuń wszystkie znaki niebędące cyframi
      const cleanNip = value.replace(/\D/g, "");

      // Sprawdź czy mamy 10 cyfr
      if (cleanNip.length !== 10) return value;

      // Formatuj NIP w polskim formacie ze spacjami: XXX XXX XX XX
      return `${cleanNip.slice(0, 3)} ${cleanNip.slice(3, 6)} ${cleanNip.slice(
        6,
        8,
      )} ${cleanNip.slice(8, 10)}`;
    },
  },
  {
    field: "buyerName",
    headerName: "Nabywca",
    flex: 1,
    minWidth: 180,
  },
  {
    field: "sellerName",
    headerName: "Sprzedawca",
    flex: 1,
    minWidth: 180,
  },
  {
    field: "invoiceType",
    headerName: "Typ",
    width: 100,
    valueGetter: (value) =>
      KSeFInvoiceTypeLabels[value as keyof typeof KSeFInvoiceTypeLabels] ||
      value,
  },
  {
    field: "cycleIdentifier",
    headerName: "Identyfikator",
    width: 120,
    valueGetter: (_value, row) => {
      if (row.cycleIdentifier && row.cycleYear) {
        return `${row.cycleIdentifier}/${row.cycleYear}`;
      }
      return "—";
    },
  },
  {
    field: "source",
    headerName: "Źródło faktury",
    width: 120,
    valueGetter: (value) =>
      InvoiceSourceLabels[value as keyof typeof InvoiceSourceLabels] || value,
    renderCell: (params) => (
      <Chip
        label={params.value}
        size="small"
        color={params.row.source === "KSeF" ? "primary" : "default"}
        variant="outlined"
      />
    ),
  },
  {
    field: "location",
    headerName: "Lokalizacja",
    width: 150,
    valueGetter: (value) => value || "—",
  },
  {
    field: "invoiceNumber",
    headerName: "Numer faktury",
    width: 150,
  },
  {
    field: "invoiceDate",
    headerName: "Data wystawienia",
    width: 140,
    valueGetter: (value: string) =>
      value ? dayjs(value).format("YYYY-MM-DD") : "—",
  },
  {
    field: "paymentDueDate",
    headerName: "Termin płatności",
    width: 140,
    valueGetter: (value: string | null) =>
      value ? dayjs(value).format("YYYY-MM-DD") : "—",
  },
  {
    field: "paymentDate",
    headerName: "Data płatności",
    width: 140,
    valueGetter: (value: string | null) =>
      value ? dayjs(value).format("YYYY-MM-DD") : "—",
  },
  {
    field: "daysUntilDue",
    headerName: "Dni do terminu płatności",
    width: 180,
    type: "number",
    valueGetter: (value: number | null) => {
      if (value === null || value === undefined) return "—";
      return value;
    },
    renderCell: (params) => {
      const days = params.value;
      if (days === "—" || days === null || days === undefined) return "—";

      const daysNum = Number(days);
      let color = "inherit";
      let fontWeight = "normal";

      if (daysNum < 0) {
        // Sprawdź czy faktura jest opłacona
        const paymentStatus = params.row.paymentStatus;
        if (
          paymentStatus === KSeFPaymentStatus.PaidCash ||
          paymentStatus === KSeFPaymentStatus.PaidTransfer
        ) {
          return "—";
        }
        // Przeterminowane - czerwony
        color = "#d32f2f";
        fontWeight = "bold";
      } else if (daysNum <= 3) {
        // 0-3 dni - pomarańczowy
        color = "#ed6c02";
        fontWeight = "bold";
      } else if (daysNum <= 7) {
        // 4-7 dni - żółty
        color = "#f57c00";
      }

      return (
        <span style={{ color, fontWeight }}>
          {daysNum < 0
            ? `${Math.abs(daysNum)} dni po terminie`
            : `${daysNum} dni`}
        </span>
      );
    },
  },
  {
    field: "moduleType",
    headerName: "Moduł",
    width: 150,
    valueGetter: (value) => {
      if (!value) return "—";
      return ModuleTypeLabels[value as keyof typeof ModuleTypeLabels] || value;
    },
  },
  {
    field: "status",
    headerName: "Status faktury",
    width: 150,
    renderCell: (params) => (
      <Chip
        label={
          KSeFInvoiceStatusLabels[params.value as KSeFInvoiceStatus] ||
          params.value
        }
        size="small"
        color={getStatusColor(
          params.value as KSeFInvoiceStatus,
          params.row.invoiceDate,
        )}
      />
    ),
  },
  {
    field: "paymentStatus",
    headerName: "Status płatności",
    width: 150,
    renderCell: (params) => (
      <Chip
        label={
          KSeFPaymentStatusLabels[params.value as KSeFPaymentStatus] ||
          params.value
        }
        size="small"
        color={getPaymentStatusColor(params.value as KSeFPaymentStatus)}
        variant="outlined"
      />
    ),
  },
  {
    field: "paymentType",
    headerName: "Typ płatności",
    width: 120,
    valueGetter: (value) =>
      KSeFInvoicePaymentTypeLabels[
        value as keyof typeof KSeFInvoicePaymentTypeLabels
      ] ||
      value ||
      "—",
  },
  {
    field: "vatDeductionType",
    headerName: "Odliczenie VAT",
    width: 120,
    valueGetter: (value) =>
      VatDeductionTypeLabels[value as VatDeductionType] || value || "—",
  },
  {
    field: "assignedUserName",
    headerName: "Przypisany pracownik",
    width: 180,
    valueGetter: (value) => value || "—",
  },
  {
    field: "comment",
    headerName: "Komentarz",
    width: 200,
    valueGetter: (value) => value || "—",
    renderCell: (params) => (
      <Tooltip title={params.value || ""}>
        <span className="truncate">{params.value}</span>
      </Tooltip>
    ),
  },
  {
    field: "grossAmount",
    headerName: "Brutto [zł]",
    width: 120,
    type: "number",
    headerAlign: "left",
    align: "left",
    valueFormatter: (value: number) =>
      value?.toLocaleString("pl-PL", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }) ?? "—",
  },
  {
    field: "quantity",
    headerName: "Ilość",
    width: 100,
    type: "number",
    headerAlign: "left",
    align: "left",
    valueFormatter: (value: number | null) =>
      value?.toLocaleString("pl-PL", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }) ?? "—",
  },
  {
    field: "dateCreatedUtc",
    headerName: "Data utworzenia wpisu",
    width: 180,
    valueGetter: (value: string) =>
      value ? dayjs(value).format("YYYY-MM-DD HH:mm") : "—",
  },
  {
    field: "actions",
    type: "actions",
    headerName: "Akcje",
    width: 140,
    getActions: (params) => {
      const isDownloading = downloadingId === params.row.id;
      const actions = [
        <Tooltip title="Pobierz PDF" key="pdf">
          <span>
            <IconButton
              size="small"
              onClick={() => onDownloadPdf(params.row)}
              disabled={isDownloading}
              color="error"
            >
              <MdPictureAsPdf />
            </IconButton>
          </span>
        </Tooltip>,
        <Tooltip title="Pobierz XML (KSeF)" key="xml">
          <span>
            <IconButton
              size="small"
              onClick={() => onDownloadXml(params.row)}
              disabled={!params.row.hasXml || isDownloading}
              color="secondary"
            >
              <MdCode />
            </IconButton>
          </span>
        </Tooltip>,
      ];

      // Add delete button only for non-KSeF invoices (source !== "KSeF")
      if (params.row.source !== "KSeF" && onDelete) {
        actions.push(
          <Tooltip title="Usuń fakturę" key="delete">
            <span>
              <IconButton
                size="small"
                onClick={() => onDelete(params.row)}
                disabled={isDownloading}
                color="error"
              >
                <MdDeleteForever />
              </IconButton>
            </span>
          </Tooltip>,
        );
      }

      return actions;
    },
  },
];
