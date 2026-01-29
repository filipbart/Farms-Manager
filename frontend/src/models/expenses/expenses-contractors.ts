import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface ExpenseTypeSimple {
  id: string;
  name: string;
}

export interface ExpenseContractorRow extends AuditFields {
  id: string;
  name: string;
  expenseTypes: ExpenseTypeSimple[];
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
  expenseTypeIds: string[];
}

export interface ExpensesContractorsFilterPaginationModel {
  searchPhrase?: string;
}
