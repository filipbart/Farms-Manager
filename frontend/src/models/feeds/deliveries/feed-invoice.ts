export interface FeedInvoiceData {
  farmId?: string;
  cycleId?: string;
  henhouseId?: string;
  identifierDisplay?: string;
  invoiceNumber?: string;
  bankAccountNumber?: string;
  vendorName?: string;
  itemName?: string;
  quantity?: number;
  unitPrice?: number;
  invoiceDate?: string;
  dueDate?: string;
  invoiceTotal?: number;
  subTotal?: number;
  vatAmount?: number;
  comment?: string;

  customerName?: string;
  nip?: string;
  henhouseName?: string;
}

export interface SaveFeedInvoiceDto {
  fileUrl: string;
  draftId: string;
  data: FeedInvoiceData;
}

import type { AuditFields } from "../../../common/interfaces/audit-fields";

export interface FeedDeliveryListModel extends AuditFields {
  id: string;
  cycleId: string;
  cycleText: string;
  farmId: string;
  farmName: string;
  henhouseId: string;
  henhouseName: string;
  vendorName: string;
  itemName: string;
  quantity?: number;
  unitPrice?: number;
  invoiceDate: string;
  dueDate?: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  comment: string;
  dateCreatedUtc: string;

  invoiceNumber: string;
  bankAccountNumber: string;

  correctUnitPrice?: number;
  paymentDateUtc?: string;
  filePath?: string;
  isCorrection: boolean;
  priority?: "Low" | "Medium" | "High";
}
