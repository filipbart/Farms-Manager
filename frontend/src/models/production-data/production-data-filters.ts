import type { CycleDictModel, FarmDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

const LOCAL_STORAGE_KEY = "productionDataFilters";

const saveFiltersToLocalStorage = (filters: ProductionDataFilterPaginationModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<ProductionDataFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): ProductionDataFilterPaginationModel => {
  const defaultFilters: ProductionDataFilterPaginationModel = {
    farmIds: [],
    cycles: [],
    henhouseIds: [],
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
  state: ProductionDataFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof ProductionDataFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ProductionDataFilterPaginationModel>;
      }
): ProductionDataFilterPaginationModel {
  let newState: ProductionDataFilterPaginationModel;

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

export enum ProductionDataOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  DeadCount = "DeadCount",
  DefectiveCount = "DefectiveCount",
  DateCreatedUtc = "DateCreatedUtc",
}

export const mapProductionDataOrderTypeToField = (
  orderType: ProductionDataOrderType
): string => {
  switch (orderType) {
    case ProductionDataOrderType.Cycle:
      return "cycleText";
    case ProductionDataOrderType.Farm:
      return "farmName";
    case ProductionDataOrderType.Henhouse:
      return "henhouseName";
    case ProductionDataOrderType.DeadCount:
      return "deadCount";
    case ProductionDataOrderType.DefectiveCount:
      return "defectiveCount";
    case ProductionDataOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};

export interface ProductionDataFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface ProductionDataFilterPaginationModel
  extends ProductionDataFilter,
    OrderedPaginationParams<ProductionDataOrderType> {}

export interface ProductionDataDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}
