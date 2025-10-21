import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

const LOCAL_STORAGE_KEY = "feedsPricesFilters";

const saveFiltersToLocalStorage = (filters: FeedsPricesFilterPaginationModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<FeedsPricesFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): FeedsPricesFilterPaginationModel => {
  const defaultFilters: FeedsPricesFilterPaginationModel = {
    farmIds: [],
    feedNames: [],
    cycles: [],
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
  state: FeedsPricesFilterPaginationModel,
  action:
    | { type: "set"; key: keyof FeedsPricesFilterPaginationModel; value: any }
    | {
        type: "setMultiple";
        payload: Partial<FeedsPricesFilterPaginationModel>;
      }
): FeedsPricesFilterPaginationModel {
  let newState: FeedsPricesFilterPaginationModel;

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

export enum FeedsPricesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  PriceDate = "PriceDate",
  Price = "Price",
  Name = "Name",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsPricesFilter {
  farmIds: string[];
  feedNames: string[];
  cycles: CycleDictModel[];
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface FeedsPricesFilterPaginationModel
  extends FeedsPricesFilter,
    OrderedPaginationParams<FeedsPricesOrderType> {}
