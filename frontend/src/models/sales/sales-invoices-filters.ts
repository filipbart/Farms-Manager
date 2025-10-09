import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

const LOCAL_STORAGE_KEY = "salesInvoicesFilters";

const saveFiltersToLocalStorage = (filters: SalesInvoicesFilterPaginationModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<SalesInvoicesFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): SalesInvoicesFilterPaginationModel => {
  const defaultFilters: SalesInvoicesFilterPaginationModel = {
    farmIds: [],
    slaughterhouseIds: [],
    cycles: [],
    dateSince: "",
    dateTo: "",
    page: 0,
    pageSize: 10,
  };

  const savedFilters = loadFiltersFromLocalStorage();

  if (savedFilters) {
    return {
      ...defaultFilters,
      ...savedFilters,

      page: 0,
    };
  }

  return defaultFilters;
};

export const initialFilters = getInitialFilters();

export function filterReducer(
  state: SalesInvoicesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof SalesInvoicesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<SalesInvoicesFilterPaginationModel>;
      }
): SalesInvoicesFilterPaginationModel {
  let newState: SalesInvoicesFilterPaginationModel;

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

export enum SalesInvoicesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Slaughterhouse = "Slaughterhouse",
  InvoiceNumber = "InvoiceNumber",
  InvoiceDate = "InvoiceDate",
  DueDate = "DueDate",
  InvoiceTotal = "InvoiceTotal",
  SubTotal = "SubTotal",
  VatAmount = "VatAmount",
}

export const mapSalesInvoiceOrderTypeToField = (
  orderType: SalesInvoicesOrderType
): string => {
  switch (orderType) {
    case SalesInvoicesOrderType.Cycle:
      return "cycleText";
    case SalesInvoicesOrderType.Farm:
      return "farmName";
    case SalesInvoicesOrderType.Slaughterhouse:
      return "slaughterhouseName";
    case SalesInvoicesOrderType.InvoiceNumber:
      return "invoiceNumber";
    case SalesInvoicesOrderType.InvoiceDate:
      return "invoiceDate";
    case SalesInvoicesOrderType.DueDate:
      return "dueDate";
    case SalesInvoicesOrderType.InvoiceTotal:
      return "invoiceTotal";
    case SalesInvoicesOrderType.SubTotal:
      return "subTotal";
    case SalesInvoicesOrderType.VatAmount:
      return "vatAmount";
    default:
      return "";
  }
};

export interface SalesInvoicesFilter {
  farmIds: string[];
  slaughterhouseIds: string[];
  cycles: CycleDictModel[];
  dateSince: string;
  dateTo: string;
}

export interface SalesInvoicesFilterPaginationModel
  extends SalesInvoicesFilter,
    OrderedPaginationParams<SalesInvoicesOrderType> {}
