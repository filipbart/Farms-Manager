import type { PaginateModel } from "../../../common/interfaces/paginate";
import type { AdvanceType } from "./categories";
import type { AuditFields } from "../../../common/interfaces/audit-fields";

export interface ExpenseAdvanceListModel extends AuditFields {
  id: string;
  date: string;
  type: AdvanceType;
  typeDesc: string;
  name: string;
  amount: number;
  categoryName: string;
  comment?: string;
  filePath?: string;
  dateCreatedUtc: string;
  priority?: "Low" | "Medium" | "High";
}

// Interfejs dla kompletnej odpowiedzi z API
export interface GetExpensesAdvancesResponse {
  employeeFullName: string;
  list: PaginateModel<ExpenseAdvanceListModel>;
  totalBalance: number;
  balance: number;
  totalIncome: number;
  totalExpenses: number;
}

export interface ExpenseAdvanceEntry {
  date: string;
  name: string;
  amount: number;
  categoryName: string;
  comment?: string;
  file?: File;
}

export interface AddExpenseAdvance {
  type: AdvanceType;
  entries: ExpenseAdvanceEntry[];
}

export interface UpdateExpenseAdvance {
  date: string;
  type: AdvanceType;
  name: string;
  amount: number;
  categoryName: string;
  comment?: string;
  file?: File;
}
