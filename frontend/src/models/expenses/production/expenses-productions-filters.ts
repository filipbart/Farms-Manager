import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

const LOCAL_STORAGE_KEY = "expensesProductionsFilters";

const saveFiltersToLocalStorage = (
  filters: ExpensesProductionsFilterPaginationModel
) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): ExpensesProductionsFilterPaginationModel | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

export const defaultFilters: ExpensesProductionsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  contractorIds: [],
  expensesTypesIds: [],
  dateSince: "",
  dateTo: "",
  invoiceNumber: "",
  page: 0,
  pageSize: 10,
};

const savedFilters = loadFiltersFromLocalStorage();

export const initialFilters: ExpensesProductionsFilterPaginationModel = {
  ...defaultFilters,
  ...savedFilters,
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
  saveFiltersToLocalStorage(newState);
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
}

export interface ExpensesProductionsFilterPaginationModel
  extends ExpensesProductionsFilter,
    OrderedPaginationParams<ExpensesProductionsOrderType> {}
