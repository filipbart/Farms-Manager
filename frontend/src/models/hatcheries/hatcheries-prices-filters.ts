import type { OrderedPaginationParams } from "../common/pagination-params";

const LOCAL_STORAGE_KEY = "hatcheriesPricesFilters";

const saveFiltersToLocalStorage = (
  filters: HatcheriesPricesFilterPaginationModel
) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<HatcheriesPricesFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): HatcheriesPricesFilterPaginationModel => {
  const defaultFilters: HatcheriesPricesFilterPaginationModel = {
    hatcheryNames: [],
    dateSince: "",
    dateTo: "",
    showDeleted: false,
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
  state: HatcheriesPricesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof HatcheriesPricesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<HatcheriesPricesFilterPaginationModel>;
      }
): HatcheriesPricesFilterPaginationModel {
  let newState: HatcheriesPricesFilterPaginationModel;

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

export enum HatcheriesPricesOrderType {
  HatcheryName = "HatcheryName",
  Price = "Price",
  Date = "Date",
}

export const mapHatcheriesPricesOrderTypeToField = (
  orderType: HatcheriesPricesOrderType
): string => {
  switch (orderType) {
    case HatcheriesPricesOrderType.HatcheryName:
      return "hatcheryName";
    case HatcheriesPricesOrderType.Price:
      return "price";
    case HatcheriesPricesOrderType.Date:
      return "date";
    default:
      return "";
  }
};

export default interface HatcheriesPricesFilter {
  hatcheryNames: string[];
  dateSince: string;
  dateTo: string;
  priceFrom?: number;
  priceTo?: number;
  showDeleted?: boolean;
}

export interface HatcheriesPricesFilterPaginationModel
  extends HatcheriesPricesFilter,
    OrderedPaginationParams<HatcheriesPricesOrderType> {}
