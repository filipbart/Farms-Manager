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
  contractorName?: string;
  expenseTypeName?: string;
  expenseTypeId?: number;
  invoiceTotal?: number;
  subTotal?: number;
  vatAmount?: number;
  invoiceDate?: string;
}

export interface DraftExpenseInvoice {
  draftId: string;
  filePath: string;
  fileUrl: string;
  extractedFields: ExpenseInvoiceData;
}
