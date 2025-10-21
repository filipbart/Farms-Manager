import type {
  CycleDictModel,
  DictModel,
  FarmDictModel,
} from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

const LOCAL_STORAGE_KEY = "productionDataFlockLossFilters";

const saveFiltersToLocalStorage = (
  filters: ProductionDataFlockLossFilterPaginationModel
) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<ProductionDataFlockLossFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): ProductionDataFlockLossFilterPaginationModel => {
  const defaultFilters: ProductionDataFlockLossFilterPaginationModel = {
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
  state: ProductionDataFlockLossFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof ProductionDataFlockLossFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ProductionDataFlockLossFilterPaginationModel>;
      }
): ProductionDataFlockLossFilterPaginationModel {
  let newState: ProductionDataFlockLossFilterPaginationModel;

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

export enum ProductionDataFlockLossOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  Hatchery = "Hatchery",
  FlockLoss1Percentage = "FlockLoss1Percentage",
  FlockLoss2Percentage = "FlockLoss2Percentage",
  FlockLoss3Percentage = "FlockLoss3Percentage",
  FlockLoss4Percentage = "FlockLoss4Percentage",
  DateCreatedUtc = "DateCreatedUtc",
}

export const mapProductionDataFlockLossOrderTypeToField = (
  orderType: ProductionDataFlockLossOrderType
): string => {
  switch (orderType) {
    case ProductionDataFlockLossOrderType.Cycle:
      return "cycleText";
    case ProductionDataFlockLossOrderType.Farm:
      return "farmName";
    case ProductionDataFlockLossOrderType.Henhouse:
      return "henhouseName";
    case ProductionDataFlockLossOrderType.Hatchery:
      return "hatcheryName";
    case ProductionDataFlockLossOrderType.FlockLoss1Percentage:
      return "flockLoss1Percentage";
    case ProductionDataFlockLossOrderType.FlockLoss2Percentage:
      return "flockLoss2Percentage";
    case ProductionDataFlockLossOrderType.FlockLoss3Percentage:
      return "flockLoss3Percentage";
    case ProductionDataFlockLossOrderType.FlockLoss4Percentage:
      return "flockLoss4Percentage";
    case ProductionDataFlockLossOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};

export interface ProductionDataFlockLossFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  hatcheryIds: string[];
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface ProductionDataFlockLossFilterPaginationModel
  extends ProductionDataFlockLossFilter,
    OrderedPaginationParams<ProductionDataFlockLossOrderType> {}

export interface ProductionDataFlockLossDictionary {
  farms: FarmDictModel[];
  hatcheries: DictModel[];
  cycles: CycleDictModel[];
}
