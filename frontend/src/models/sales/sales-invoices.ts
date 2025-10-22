import type { AuditFields } from "../../common/interfaces/audit-fields";

export enum SalesInvoiceStatus {
  Unrealized = "Unrealized",
  Realized = "Realized",
}

export const SalesInvoiceStatusLabels: Record<SalesInvoiceStatus, string> = {
  [SalesInvoiceStatus.Unrealized]: "Niezrealizowany",
  [SalesInvoiceStatus.Realized]: "Zrealizowany",
};

export interface SalesInvoiceListModel extends AuditFields {
  id: string;
  cycleId: string;
  cycleText: string;
  farmId: string;
  farmName: string;
  slaughterhouseName: string;
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  dateCreatedUtc: string;
  filePath?: string;
  paymentDate?: string;
  priority?: "Low" | "Medium" | "High";
  status?: SalesInvoiceStatus;
  comment?: string;
}

export interface UpdateSalesInvoiceData {
  cycleId: string;
  invoiceNumber: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  invoiceDate: string;
  dueDate: string;
  paymentDate?: string;
}

export interface SalesInvoiceData {
  farmId?: string;
  cycleId?: string;
  cycleDisplay?: string;
  slaughterhouseId?: string;
  slaughterhouseName?: string;
  invoiceNumber?: string;
  invoiceDate?: string;
  dueDate?: string;
  invoiceTotal?: number;
  subTotal?: number;
  vatAmount?: number;
}

export interface SaveSalesInvoiceData {
  filePath: string;
  draftId: string;
  data: SalesInvoiceData;
}

export interface DraftSalesInvoice {
  draftId: string;
  filePath: string;
  fileUrl: string;
  extractedFields: SalesInvoiceData;
}

export interface UploadSalesInvoicesFilesResponse {
  files: DraftSalesInvoice[];
}
