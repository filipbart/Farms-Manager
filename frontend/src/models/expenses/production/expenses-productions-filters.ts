import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: ExpensesProductionsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  contractorIds: [],
  expensesTypesIds: [],
  dateSince: "",
  dateTo: "",
  invoiceNumber: "",
  showDeleted: false,
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: ExpensesProductionsFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof ExpensesProductionsFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ExpensesProductionsFilterPaginationModel>;
      }
): ExpensesProductionsFilterPaginationModel {
  let newState: ExpensesProductionsFilterPaginationModel;
  switch (action.type) {
    case "set":
      newState = { ...state, [action.key]: action.value };
      break;
    case "setMultiple":
      newState = { ...state, ...action.payload };
      break;
    default:
      return state;
  }
  return newState;
}

export enum ExpensesProductionsOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Contractor = "Contractor",
  ExpenseType = "ExpenseType",
  InvoiceTotal = "InvoiceTotal",
  SubTotal = "SubTotal",
  VatAmount = "VatAmount",
  InvoiceDate = "InvoiceDate",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface ExpensesProductionsFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  contractorIds: string[];
  expensesTypesIds: string[];
  dateSince: string;
  dateTo: string;
  invoiceNumber: string;
  showDeleted?: boolean;
}

export interface ExpensesProductionsFilterPaginationModel
  extends ExpensesProductionsFilter,
    OrderedPaginationParams<ExpensesProductionsOrderType> {}
