import type { AuditFields } from "../../../common/interfaces/audit-fields";

export interface ExpenseProductionListModel extends AuditFields {
  id: string;
  farmId: string;
  cycleId: string;
  expenseContractorId: string;
  cycleText: string;
  farmName: string;
  contractorName: string;
  invoiceNumber: string;
  expenseTypeName: string;
  expenseTypeId?: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  invoiceDate: string;
  dateCreatedUtc: string;
  filePath?: string;
  comment?: string;
}

export interface AddExpenseProductionData {
  farmId: string;
  cycleId: string;
  cycleDisplay: string;
  expenseContractorId: string;
  expenseTypeNameDisplay: string;
  invoiceNumber: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  invoiceDate: string;
  comment?: string;
  file?: File;
}

export interface UpdateExpenseProductionData {
  farmId: string;
  cycleId: string;
  expenseContractorId: string;
  expenseTypeId?: string;
  invoiceNumber: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  invoiceDate: string;
  cycleDisplay?: string;
  expenseTypeNameDisplay?: string;
  comment?: string;
}
export interface ExpenseInvoiceData {
  farmId?: string;
  cycleId?: string;
  cycleDisplay?: string;
  invoiceNumber?: string;
  contractorId?: string;
  contractorName?: string;
  expenseTypeName?: string;
  expenseTypeId?: string;
  invoiceTotal?: number;
  subTotal?: number;
  vatAmount?: number;
  invoiceDate?: string;
  comment?: string;
}

export interface SaveExpenseInvoiceData {
  filePath: string;
  draftId: string;
  data: ExpenseInvoiceData;
}

export interface DraftExpenseInvoice {
  draftId: string;
  filePath: string;
  fileUrl: string;
  extractedFields: ExpenseInvoiceData;
}
