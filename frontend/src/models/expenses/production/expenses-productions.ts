export interface ExpenseProductionListModel {
  id: string;
  cycleText: string;
  farmName: string;
  contractorName: string;
  invoiceNumber: string;
  expenseTypeName: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  invoiceDate: string;
  dateCreatedUtc: string;
  filePath?: string;
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
  file?: File;
}

export interface UpdateExpenseProductionData {
  invoiceNumber: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  invoiceDate: string;
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
