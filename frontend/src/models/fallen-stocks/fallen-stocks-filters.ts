import type { FarmDictModel, CycleDictModel } from "../common/dictionaries";

const LOCAL_STORAGE_KEY = "fallenStocksFilters";

const saveFiltersToLocalStorage = (filters: FallenStockFilterModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<FallenStockFilterModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): FallenStockFilterModel => {
  const defaultFilters: FallenStockFilterModel = {
    farmId: undefined,
    cycle: undefined,
  };

  const savedFilters = loadFiltersFromLocalStorage();

  if (savedFilters) {
    return {
      ...defaultFilters,
      ...savedFilters,
    };
  }

  return defaultFilters;
};

export const initialFilters = getInitialFilters();

export function filterReducer(
  state: FallenStockFilterModel,
  action:
    | { type: "set"; key: keyof FallenStockFilterModel; value: any }
    | { type: "setMultiple"; payload: Partial<FallenStockFilterModel> }
): FallenStockFilterModel {
  let newState: FallenStockFilterModel;

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

export interface FallenStocksDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}

export interface FallenStockFilterModel {
  farmId?: string;
  cycle?: string;
}
