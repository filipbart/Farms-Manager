import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: ExpensesProductionsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  contractorIds: [],
  expensesTypesIds: [],
  dateSince: "",
  dateTo: "",
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
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
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
}

export interface ExpensesProductionsFilterPaginationModel
  extends ExpensesProductionsFilter,
    OrderedPaginationParams<ExpensesProductionsOrderType> {}
