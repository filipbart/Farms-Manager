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
}
