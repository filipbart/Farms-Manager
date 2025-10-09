import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

const LOCAL_STORAGE_KEY = "salesFilters";

const saveFiltersToLocalStorage = (filters: SalesFilterPaginationModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<SalesFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): SalesFilterPaginationModel => {
  const defaultFilters: SalesFilterPaginationModel = {
    farmIds: [],
    cycles: [],
    henhouseIds: [],
    slaughterhouseIds: [],
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
  state: SalesFilterPaginationModel,
  action:
    | { type: "set"; key: keyof SalesFilterPaginationModel; value: any }
    | { type: "setMultiple"; payload: Partial<SalesFilterPaginationModel> }
): SalesFilterPaginationModel {
  let newState: SalesFilterPaginationModel;

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

export enum SalesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  SaleDate = "SaleDate",
  //   WeightSlaughter = "WeightSlaughter",
  //   QuantitySlaughter = "QuantitySlaughter",
  DateCreatedUtc = "DateCreatedUtc",
}

interface SalesFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  slaughterhouseIds: string[];
  dateSince: string;
  dateTo: string;
}

export interface SalesFilterPaginationModel
  extends SalesFilter,
    OrderedPaginationParams<SalesOrderType> {}
