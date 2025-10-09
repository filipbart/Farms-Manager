import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

const LOCAL_STORAGE_KEY = "feedsDeliveriesFilters";

const saveFiltersToLocalStorage = (filters: FeedsDeliveriesFilterPaginationModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<FeedsDeliveriesFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): FeedsDeliveriesFilterPaginationModel => {
  const defaultFilters: FeedsDeliveriesFilterPaginationModel = {
    farmIds: [],
    henhouseIds: [],
    feedNames: [],
    invoiceNumber: "",
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
  state: FeedsDeliveriesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof FeedsDeliveriesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<FeedsDeliveriesFilterPaginationModel>;
      }
): FeedsDeliveriesFilterPaginationModel {
  let newState: FeedsDeliveriesFilterPaginationModel;

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

export enum FeedsDeliveriesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  HenhouseName = "HenhouseName",
  ItemName = "ItemName",
  VendorName = "VendorName",
  InvoiceNumber = "InvoiceNumber",
  Quantity = "Quantity",
  UnitPrice = "UnitPrice",
  InvoiceDate = "InvoiceDate",
  DueDate = "DueDate",
  InvoiceTotal = "InvoiceTotal",
  SubTotal = "SubTotal",
  VatAmount = "VatAmount",
  PaymentDateUtc = "PaymentDateUtc",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsDeliveriesFilter {
  farmIds: string[];
  henhouseIds: string[];
  feedNames: string[];
  invoiceNumber: string;
  cycles: CycleDictModel[];
  incorrectPrices?: boolean;
  dateSince: string;
  dateTo: string;
}

export interface FeedsDeliveriesFilterPaginationModel
  extends FeedsDeliveriesFilter,
    OrderedPaginationParams<FeedsDeliveriesOrderType> {}
