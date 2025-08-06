export enum AdvanceType {
  Income = "Income",
  Expense = "Expense",
}

export interface AdvanceCategoryModel {
  id: string;
  type: AdvanceType;
  typeDesc: string;
  name: string;
}

export interface AddAdvanceCategory {
  type: AdvanceType;
  name: string;
}
