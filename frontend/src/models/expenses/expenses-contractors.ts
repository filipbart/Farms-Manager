export interface ExpenseContractorRow {
  id: string;
  name: string;
  expenseType: string;
  expenseTypeId: string;
  nip: string;
  address: string;
  dateCreatedUtc: string;
}

export interface ExpensesContractorsQueryResponse {
  contractors: ExpenseContractorRow[];
}

export interface AddExpenseContractorData {
  name: string;
  nip: string;
  address: string;
  expenseTypeId: string;
  expenseType: string;
}

export interface ExpensesContractorsFilterPaginationModel {
  searchPhrase?: string;
}
