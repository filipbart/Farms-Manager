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
