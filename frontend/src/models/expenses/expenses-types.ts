export interface ExpenseTypeRow {
  id: string;
  name: string;
}

export interface ExpensesTypesQueryResponse {
  types: ExpenseTypeRow[];
}
