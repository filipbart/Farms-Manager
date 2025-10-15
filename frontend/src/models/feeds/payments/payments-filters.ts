import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

const LOCAL_STORAGE_KEY = "feedsPaymentsFilters";

const saveFiltersToLocalStorage = (filters: FeedsPaymentsFilterPaginationModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<FeedsPaymentsFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): FeedsPaymentsFilterPaginationModel => {
  const defaultFilters: FeedsPaymentsFilterPaginationModel = {
    farmIds: [],
    cycles: [],
    statuses: undefined,
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
  state: FeedsPaymentsFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof FeedsPaymentsFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<FeedsPaymentsFilterPaginationModel>;
      }
): FeedsPaymentsFilterPaginationModel {
  let newState: FeedsPaymentsFilterPaginationModel;

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

export enum FeedsPaymentsOrderType {
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsPaymentsFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  statuses?: string;
}

export interface FeedsPaymentsFilterPaginationModel
  extends FeedsPaymentsFilter,
    OrderedPaginationParams<FeedsPaymentsOrderType> {}
