import {
  ModuleType,
  KSeFPaymentStatus,
  KSeFInvoiceStatus,
  VatDeductionType,
  InvoiceDocumentType,
} from "../../../../models/accounting/ksef-invoice";

export interface InvoiceFormData {
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  sellerName: string;
  sellerNip: string;
  buyerName: string;
  buyerNip: string;
  grossAmount: number;
  netAmount: number;
  vatAmount: number;
  documentType: InvoiceDocumentType;
  status: KSeFInvoiceStatus;
  vatDeductionType: VatDeductionType;
  moduleType: ModuleType;
  paymentStatus: KSeFPaymentStatus;
  paymentDate: string;
  comment: string;
  assignedUserId: string;
  relatedInvoiceNumber: string;
  // Feed module fields
  feedFarmId: string;
  feedCycleId: string;
  feedHenhouseId: string;
  feedBankAccountNumber: string;
  feedVendorName: string;
  feedItemName: string;
  feedQuantity: string | number;
  feedUnitPrice: string | number;
  // Gas module fields
  gasFarmId: string;
  gasContractorId: string;
  gasUnitPrice: string | number;
  gasQuantity: string | number;
  // Expense module fields
  expenseFarmId: string;
  expenseCycleId: string;
  expenseContractorId: string;
  expenseTypeId: string;
  // Sale module fields
  saleFarmId: string;
  saleCycleId: string;
  saleSlaughterhouseId: string;
}

export const getFarmIdForModule = (formData: InvoiceFormData): string => {
  switch (formData.moduleType) {
    case ModuleType.Feeds:
      return formData.feedFarmId;
    case ModuleType.Gas:
      return formData.gasFarmId;
    case ModuleType.ProductionExpenses:
      return formData.expenseFarmId;
    case ModuleType.Sales:
      return formData.saleFarmId;
    default:
      return "";
  }
};

export const getCycleIdForModule = (formData: InvoiceFormData): string => {
  switch (formData.moduleType) {
    case ModuleType.Feeds:
      return formData.feedCycleId;
    case ModuleType.ProductionExpenses:
      return formData.expenseCycleId;
    case ModuleType.Sales:
      return formData.saleCycleId;
    default:
      return "";
  }
};
