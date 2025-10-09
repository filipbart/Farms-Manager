import type { OrderedPaginationParams } from "../../common/pagination-params";

const LOCAL_STORAGE_KEY = "expensesAdvancesFilters";

const saveFiltersToLocalStorage = (filters: ExpensesAdvancesFilterPaginationModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): ExpensesAdvancesFilterPaginationModel | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

export const defaultFilters: ExpensesAdvancesFilterPaginationModel = {
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

const savedFilters = loadFiltersFromLocalStorage();

export const initialFilters: ExpensesAdvancesFilterPaginationModel = {
  ...defaultFilters,
  ...savedFilters,
};

export function filterReducer(
  state: ExpensesAdvancesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof ExpensesAdvancesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ExpensesAdvancesFilterPaginationModel>;
      }
): ExpensesAdvancesFilterPaginationModel {
  let newState: ExpensesAdvancesFilterPaginationModel;
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

export const mapExpensesAdvancesOrderTypeToField = (
  orderType: ExpensesAdvancesOrderType
): string => {
  switch (orderType) {
    case ExpensesAdvancesOrderType.Date:
      return "date";
    case ExpensesAdvancesOrderType.Type:
      return "type";
    case ExpensesAdvancesOrderType.Name:
      return "name";
    case ExpensesAdvancesOrderType.Amount:
      return "amount";
    case ExpensesAdvancesOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};

export enum ExpensesAdvancesOrderType {
  Date = "Date",
  Type = "Type",
  Name = "Name",
  Amount = "Amount",
  DateCreatedUtc = "DateCreatedUtc",
}

export interface ExpensesAdvancesFilter {
  dateSince: string;
  dateTo: string;
}

export interface ExpensesAdvancesFilterPaginationModel
  extends ExpensesAdvancesFilter,
    OrderedPaginationParams<ExpensesAdvancesOrderType> {
  years?: number[];
  months?: number[];
}
