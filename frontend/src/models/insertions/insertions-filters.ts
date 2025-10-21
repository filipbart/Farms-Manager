import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

const LOCAL_STORAGE_KEY = "insertionsFilters";

const saveFiltersToLocalStorage = (
  filters: InsertionsFilterPaginationModel
) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<InsertionsFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): InsertionsFilterPaginationModel => {
  const defaultFilters: InsertionsFilterPaginationModel = {
    farmIds: [],
    cycles: [],
    henhouseIds: [],
    hatcheryIds: [],
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
  state: InsertionsFilterPaginationModel,
  action:
    | { type: "set"; key: keyof InsertionsFilterPaginationModel; value: any }
    | { type: "setMultiple"; payload: Partial<InsertionsFilterPaginationModel> }
): InsertionsFilterPaginationModel {
  let newState: InsertionsFilterPaginationModel;

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

export enum InsertionOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  InsertionDate = "InsertionDate",
  Quantity = "Quantity",
  Hatchery = "Hatchery",
  BodyWeight = "BodyWeight",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface InsertionsFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  hatcheryIds: string[];
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface InsertionsFilterPaginationModel
  extends InsertionsFilter,
    OrderedPaginationParams<InsertionOrderType> {}
