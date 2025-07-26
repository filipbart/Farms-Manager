import { ExpensesProductionsOrderType } from "../../models/expenses/production/expenses-productions-filters";

export const mapExpenseProductionOrderTypeToField = (
  orderType: ExpensesProductionsOrderType
): string => {
  switch (orderType) {
    case ExpensesProductionsOrderType.Cycle:
      return "cycleText";
    case ExpensesProductionsOrderType.Farm:
      return "farmName";
    case ExpensesProductionsOrderType.Contractor:
      return "contractorName";
    case ExpensesProductionsOrderType.ExpenseType:
      return "expenseTypeName";
    case ExpensesProductionsOrderType.InvoiceTotal:
      return "grossValue";
    case ExpensesProductionsOrderType.SubTotal:
      return "netValue";
    case ExpensesProductionsOrderType.VatAmount:
      return "vatValue";
    case ExpensesProductionsOrderType.InvoiceDate:
      return "invoiceDate";
    case ExpensesProductionsOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
