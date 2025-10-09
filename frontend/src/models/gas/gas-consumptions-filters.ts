import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

const LOCAL_STORAGE_KEY = "gasConsumptionsFilters";

const saveFiltersToLocalStorage = (
  filters: GasConsumptionsFilterPaginationModel
) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<GasConsumptionsFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): GasConsumptionsFilterPaginationModel => {
  const defaultFilters: GasConsumptionsFilterPaginationModel = {
    farmIds: [],
    cycles: [],
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
  state: GasConsumptionsFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof GasConsumptionsFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<GasConsumptionsFilterPaginationModel>;
      }
): GasConsumptionsFilterPaginationModel {
  let newState: GasConsumptionsFilterPaginationModel;

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

export enum GasConsumptionsOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  QuantityConsumed = "QuantityConsumed",
  Cost = "Cost",
}

export const mapGasConsumptionOrderTypeToField = (
  orderType: GasConsumptionsOrderType
): string => {
  switch (orderType) {
    case GasConsumptionsOrderType.Cycle:
      return "cycleText";
    case GasConsumptionsOrderType.Farm:
      return "farmName";
    case GasConsumptionsOrderType.QuantityConsumed:
      return "quantityConsumed";
    case GasConsumptionsOrderType.Cost:
      return "cost";
    default:
      return "";
  }
};

export interface GasConsumptionsFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
}

export interface GasConsumptionsFilterPaginationModel
  extends GasConsumptionsFilter,
    OrderedPaginationParams<GasConsumptionsOrderType> {}
