import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { Chip, IconButton, Tooltip } from "@mui/material";
import { MdPictureAsPdf, MdCode } from "react-icons/md";
import type { KSeFInvoiceListModel } from "../../models/accounting/ksef-invoice";
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
  VatDeductionType,
} from "../../models/accounting/ksef-invoice";

interface GetKSeFInvoicesColumnsProps {
  onDownloadPdf: (invoice: KSeFInvoiceListModel) => void;
  onDownloadXml: (invoice: KSeFInvoiceListModel) => void;
  downloadingId: string | null;
}

const getStatusColor = (status: KSeFInvoiceStatus) => {
  switch (status) {
    case KSeFInvoiceStatus.Accepted:
      return "success";
    case KSeFInvoiceStatus.Rejected:
      return "error";
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

export const getKSeFInvoicesColumns = ({
  onDownloadPdf,
  onDownloadXml,
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
        color={getStatusColor(params.value as KSeFInvoiceStatus)}
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
    field: "actions",
    type: "actions",
    headerName: "Akcje",
    width: 100,
    getActions: (params) => {
      const isDownloading = downloadingId === params.row.id;
      return [
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
    },
  },
];
