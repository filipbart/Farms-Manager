import type {
  CycleDictModel,
  DictModel,
  FarmDictModel,
} from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: ProductionDataFlockLossFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  hatcheryIds: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

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
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
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
}

export interface ProductionDataFlockLossFilterPaginationModel
  extends ProductionDataFlockLossFilter,
    OrderedPaginationParams<ProductionDataFlockLossOrderType> {}

export interface ProductionDataFlockLossDictionary {
  farms: FarmDictModel[];
  hatcheries: DictModel[];
  cycles: CycleDictModel[];
}
